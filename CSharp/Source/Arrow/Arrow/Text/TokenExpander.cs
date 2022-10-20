using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Arrow.Settings;

namespace Arrow.Text
{
	/// <summary>
	/// Expands text which may contain tokens
	/// </summary>
	public static class TokenExpander
	{
		/// <summary>
		/// A default begin token to use
		/// </summary>
		public static readonly string DefaultBeginToken="${";
		
		/// <summary>
		/// A default end token to use
		/// </summary>
		public static readonly string DefaultEndToken="}";
			
		private static readonly BindingFlags PropertyBindings=BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Instance;
		
		
		/// <summary>
		/// Expands a string using the default begin and end tokens
		/// </summary>
		/// <param name="value">The string to expand</param>
		/// <returns>An expanded version of the string</returns>
		public static string ExpandText(string value)
		{
			return ExpandText(value,DefaultBeginToken,DefaultEndToken);
		}
		
		/// <summary>
		/// Expands a string that may contains tokens prefixed by beginMark and terminated by endMark
		/// </summary>
		/// <param name="value">The string to examine</param>
		/// <param name="beginToken">The characters that mark the start of a token</param>
		/// <param name="endToken">The characters that mark the end of the token</param>
		/// <returns>The expanded version of value</returns>
		public static string ExpandText(string value, string beginToken, string endToken)
		{
			return ExpandText(value,beginToken,endToken,null);
		}
		
		/// <summary>
		/// Expands a string using the default begin and end tokens
		/// </summary>
		/// <param name="value">The string to expand</param>
		/// <param name="unknownVariableLookup">A function to call if the variable cannot be resolved (may be null)</param>
		/// <returns>An expanded version of the string</returns>
		public static string ExpandText(string value, Func<string,object?> unknownVariableLookup)
		{
			return ExpandText(value,DefaultBeginToken,DefaultEndToken,unknownVariableLookup);
		}
				
		/// <summary>
		/// Expands a string that may contains tokens prefixed by beginMark and terminated by endMark.
		/// If the token cannot be expanded then the optional unknownVariableLookup handler is called
		/// to give the caller the chance to resolve the value
		/// </summary>
		/// <param name="value">The string to examine</param>
		/// <param name="beginToken">The characters that mark the start of a token</param>
		/// <param name="endToken">The characters that mark the end of the token</param>
		/// <param name="unknownVariableLookup">A function to call if the variable cannot be resolved (may be null)</param>
		/// <returns>The expanded version of value</returns>
		public static string ExpandText(string value, string beginToken, string endToken, Func<string,object?>? unknownVariableLookup)
		{
			int beginMarkLength=beginToken.Length;
			int endMarkLength=endToken.Length;
			
			int index=-1;
			int startIndex=0;
			
			while((index=value.IndexOf(beginToken,startIndex))!=-1)
			{
				int end=value.IndexOf(endToken,index+1);
				if(end==-1)
				{
					break;
				}
				else
				{
					string token=value.Substring(index+beginMarkLength,(end-index)-beginMarkLength);
					string tokenValue=ExpandToken(token,unknownVariableLookup);
					
					if(tokenValue==null)
					{
						throw new ArrowException("could not resolve "+token);
					}
					
					string leftPart=value.Substring(0,index);
					string rightPart=value.Substring(end+endMarkLength);
					
					value=leftPart+tokenValue+rightPart;
					startIndex=leftPart.Length+tokenValue.Length;
				}
			}
			
			return value;
		}
		
		/// <summary>
		/// Expands a token of the form namespace:variable|formatting|action
		/// </summary>
		/// <param name="token">The token to expand</param>
		/// <returns>The value for the token</returns>
		public static string ExpandToken(string token)
		{
			return ExpandToken(token,null);
		}
		
		/// <summary>
		/// Expands a token of the form namespace:variable|formatting|action
		/// </summary>
		/// <param name="token">The token to expand</param>
		/// <param name="unknownVariableLookup">The handler to call if the variable is not found or does not have a namespace qualifier</param>
		/// <returns>The value for the token</returns>
		/// <exception cref="System.ArgumentNullException">token is null</exception>
		public static string ExpandToken(string token, Func<string,object?>? unknownVariableLookup)
		{
			if(token==null) throw new ArgumentNullException("token");
		
			string? result=null;
			
			string? @namespace=null;
			string? variable=null;
			string? property=null;
			string? formatting=null;
			object? value=null;
			string? action=null;
			
			// Split the pipeline apart to get the variable|formatting|action parts
			string[] parts=token.Split(new char[]{'|'},4);
			if(parts.Length>0) variable=parts[0];
			if(parts.Length>1) property=parts[1];
			if(parts.Length>2) formatting=parts[2];
			if(parts.Length>3) action=parts[3];
			
			if(variable==null) throw new ArrowException("token does not contain a variable: "+token);
			
			string originalVariable=variable;
			
			// See if the token comes from a namespace
			int pivot=variable.IndexOf(SettingsManager.NamespaceSeparatorChar);
			
			if(pivot==-1)
			{	
				// It's a regular variable
				@namespace=null;
			}
			else
			{
				// It's a value from a namespace
				@namespace=variable.Substring(0,pivot);
				variable=variable.Substring(pivot+1);
			}
			
			if(@namespace==null)
			{
				// If no namespace pass it to the unknown lookup
				if(unknownVariableLookup!=null)
				{
					value=unknownVariableLookup(originalVariable);
				}
			}
			else
			{
				// Get the variable from the namespace
				ISettings? settings=SettingsManager.GetSettings(@namespace);
				if(settings!=null)
				{
					value=settings.GetSetting(variable);
				}
				else
				{				
					// If it's not a namespace from the global settings then
					// give the caller a change to look up the value
					if(unknownVariableLookup!=null)
					{
						value=unknownVariableLookup(originalVariable);
					}
				}
			}
			
			// We need a value
			if(value==null) throw new ArrowException(token+" could not be resolved");
			
			// See if we're doing a property lookup on the value
			if(string.IsNullOrEmpty(property)==false)
			{
				foreach(string propertyName in property!.Split('.'))
				{
					if(value is ISettings)
					{
						// Since a settings is just a bag of values it
						// makes sense to treat is as effectivly a bunch of properties
						ISettings settings=(ISettings)value;
						value=settings.GetSetting(propertyName);
					}
					else
					{
						var info=value!.GetType().GetProperty(propertyName,PropertyBindings);
						if(info==null) throw new ArrowException("property not found: "+propertyName);
						
						var method=info.GetGetMethod();
						if(method==null) throw new ArrowException("property not readable: "+propertyName);
						
						value=method.Invoke(value,null);
					}
				}
			}
			
			// Apply any formatting if the user has specified it
			if(string.IsNullOrEmpty(formatting))
			{
				result=value.ToString()!;
			}
			else
			{
				var formattable=value as IFormattable;
				if(formattable!=null)
				{
					result=formattable.ToString(formatting,null);
				}
				else
				{
					// As the object isn't formattable just return the value
					result=value.ToString()!;
				}
			}
			
			// Apply any actions
			if(string.IsNullOrEmpty(action)==false)
			{
				switch(action!.ToLower())
				{
					case "trim":
						result=result.Trim();
						break;
					
					case "trimstart":
						result=result.TrimStart();
						break;
						
					case "trimend":
						result=result.TrimEnd();
						break;
						
					case "toupper":
						result=result.ToUpper();
						break;
						
					case "tolower":
						result=result.ToLower();
						break;
						
					default:
						throw new ArrowException("invalid action "+token);
				}
			}
			
			return result;
		}
	}
}
