using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Arrow.Settings;
using Arrow.Reflection;

namespace Arrow.ObjectAccess
{
	/// <summary>
	/// Locates an object from a uri.
	/// The uri has the format scheme://object@assembly/path/to/object
	/// By default 2 schemes are supported, "static" and "setting".
	/// "static" is able to navigate from a static member. Eg "static://Arrow.ObjectAccess.ObjectLocator@Arrow/StaticScheme/Length"
	/// Note that the assembly name is encoded as the userinfo.
	/// </summary>
	public static class ObjectLocator
	{
		/// <summary>
		/// The uri scheme to look up against a static object
		/// </summary>
		public const string StaticScheme="static";
		
		/// <summary>
		/// The uri scheme for a settings provider
		/// </summary>
		public const string SettingScheme="setting";
		
		private const BindingFlags Flags=BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Instance|BindingFlags.Static;
	
		/// <summary>
		/// Attempts to locate an object
		/// </summary>
		/// <param name="uri">The uri of the object</param>
		/// <returns>The object at the uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.ArgumentNullException">if the scheme requires a lookup instance</exception>
		public static object Locate(Uri uri)
		{
			return Locate<object>(uri,null);
		}
		
		/// <summary>
		/// Attempts to locate an object
		/// </summary>
		/// <param name="uri">The uri of the object</param>
		/// <param name="rootLookup">The delegate to convert the host to an root object, if required</param>
		/// <returns>The object at the uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.ArgumentNullException">if the scheme is not static and rootLookup is null</exception>
		public static object Locate(Uri uri, Func<string,object> rootLookup)
		{
			return Locate<object>(uri,rootLookup);
		}
	
		/// <summary>
		/// Attempts to locate an object
		/// </summary>
		/// <typeparam name="T">The minimum type required by the object</typeparam>
		/// <param name="uri">The uri of the object</param>
		/// <param name="rootLookup">The delegate to convert the host to an root object, if required</param>
		/// <returns>The object at the uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.ArgumentNullException">if the scheme is not "static" or "depend" and rootLookup is null</exception>
		public static T Locate<T>(Uri uri, Func<string,object> rootLookup)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			object obj=null;
			
			switch(uri.Scheme)
			{
				case StaticScheme:
					obj=ResolveStatic(uri);
					break;

				case SettingScheme:
					obj=ResolveViaLookup(uri,SettingsManager.GetSettings);
					break;
					
				default:
					// Anything else is assumed to require a lookup
					if(rootLookup==null) throw new ArgumentNullException("rootLookup");
					obj=ResolveViaLookup(uri,rootLookup);
					break;
			}
			
			return (T)obj;
		}
		
		/// <summary>
		/// Indicates if a scheme is supported by the object locator
		/// </summary>
		/// <param name="scheme">The scheme to check</param>
		/// <returns>true if the scheme can be handled, false otherwise</returns>
		public static bool IsSchemeSupported(string scheme)
		{
			if(scheme==null) throw new ArgumentNullException("scheme");
			
			return scheme==StaticScheme || scheme==SettingScheme;
		}
		
		private static object ResolveStatic(Uri uri)
		{
			string typeName=uri.Host;
			string assemblyName=null;
			
			// If the hostname is in the form x@y then x is the type and y is the namespace
			// We can't use , to seperate them as it's an invalid character
			if(string.IsNullOrEmpty(uri.UserInfo)==false)
			{
				assemblyName=typeName;
				typeName=uri.UserInfo;
			}
			
			Type type=(assemblyName==null ? TypeResolver.GetEncodedType(typeName) : TypeResolver.GetEncodedType(typeName,assemblyName));
			
			string[] properties=SplitPathIntoProperties(uri);
			
			// As we're static we need at least one property lookup
			if(properties==null || properties.Length==0) throw new UriFormatException("static uri must list at least one property: "+uri);
			
			return NavigateObject(type,null,properties);
		}
		
		private static object ResolveViaLookup(Uri uri, Func<string,object> rootLookup)
		{
			string startObjectName=uri.Host;
			
			object locatedObject=rootLookup(startObjectName);
			if(locatedObject==null) throw new ArrowException("root object is null: "+uri);
			
			string[] properties=SplitPathIntoProperties(uri);
			return NavigateObject(locatedObject.GetType(),locatedObject,properties);
		}
		
		/// <summary>
		/// Splts the path part of the uri into an array
		/// </summary>
		/// <param name="uri">The uri with the path to split</param>
		/// <returns>An array of path parts</returns>
		private static string[] SplitPathIntoProperties(Uri uri)
		{
			string[] path=uri.AbsolutePath.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
			return path;
		}
		
		/// <summary>
		/// Navigates through the property paths searching for the final object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="obj"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		private static object NavigateObject(Type type, object obj, string[] properties)
		{
			object locatedObject=obj;
		
			for(int i=0; i<properties.Length; i++)
			{
				string propertyName=properties[i];
				
				// On every run except the first we need the type to be the type of the located object.
				// Also, null is not permitted for any value other than the last property lookup
				if(i!=0) 
				{
					if(locatedObject==null) throw new InvalidOperationException("null encountered when more lookup specified");
					type=locatedObject.GetType();
				}
				
				locatedObject=GetValue(type,locatedObject,propertyName);
			}
			
			return locatedObject;
		}
		
		/// <summary>
		/// Determines the value of a property/field/method()
		/// </summary>
		/// <param name="type">The type to use in a reflection lookup</param>
		/// <param name="object">The object to call against. If null then the call is a static call</param>
		/// <param name="name">The name of the property/field/method to fetch/call</param>
		/// <returns>The value of the property/field/method</returns>
		private static object GetValue(Type type, object @object, string name)
		{
			// Treat setting providers as a bucket of properties
			if(@object is ISettings)
			{
				return ((ISettings)@object).GetSetting(name);
			}
		
			// We'll resolve in property/field/method order as 
			// this seems the most obvious thing a user would intend
			PropertyInfo propertyInfo=type.GetProperty(name,Flags);
			if(propertyInfo!=null)
			{
				MethodInfo getMethod=propertyInfo.GetGetMethod();
				if(getMethod==null) throw new ArrowException("property not readable: "+name);
				
				@object=getMethod.Invoke(@object,null);
			}
			else
			{
				// It might be a public field
				FieldInfo fieldInfo=type.GetField(name,Flags);
				if(fieldInfo!=null)
				{				
					@object=fieldInfo.GetValue(@object);
				}
				else
				{
					// Finally, try a method of the form: RetType Method()
					MethodInfo methodInfo=type.GetMethod(name,Flags,null,Type.EmptyTypes,null);
					if(methodInfo!=null)
					{
						if(methodInfo.ReturnType==typeof(void)) throw new ArrowException("resolved method returns void: "+name);
						if(methodInfo.IsGenericMethod) throw new ArrowException("resolved method is generic: "+name);
						
						@object=methodInfo.Invoke(@object,null);
					}
					else
					{
						throw new ArrowException("could not resolve "+name+"()");
					}
				}
			}
			
			return @object;
		}
		
		/// <summary>
		/// Returns a lookup object that resolves the root by lookup up
		/// a property/field on the supplied object
		/// </summary>
		/// <param name="object">The object to lookup against</param>
		/// <returns>A lookup delegate</returns>
		/// <exception cref="System.ArgumentNullException">@object is null</exception>
		public static Converter<string,object> MakeObjectRootLookup(object @object)
		{
			if(@object==null) throw new ArgumentNullException("object");
		
			return (string name)=>
			{
				return GetValue(@object.GetType(),@object,name);
			};
		}
	}
}
