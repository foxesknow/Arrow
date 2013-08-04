using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Xml;

using Arrow.Serialization;
using Arrow.Storage;

namespace Arrow.Data
{
	/// <summary>
	/// Useful data model class for type representing database rows
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class DataModel<T> where T:new()
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected DataModel()
		{
		}
		
		/// <summary>
		/// Implements cloning be serializing the class to a memory stream and back again
		/// </summary>
		/// <returns>A deep copy of the instance</returns>
		public virtual T Clone()
		{
			using(MemoryStream stream=new MemoryStream())
			{
				GenericBinaryFormatter formatter=new GenericBinaryFormatter();
				formatter.Serialize(stream,this);
				stream.Position=0;
				return formatter.Deserialize<T>(stream);
			}
		}
		/// <summary>
		/// Loads a script from an xml resource.
		/// The name of the root element is not important, but each script
		/// must be in a <b>Script</b> element and have a <b>name</b> attribute
		/// </summary>
		/// <param name="path">The path to the xml resource</param>
		/// <param name="scriptName">The name of the script within the xml resource</param>
		/// <returns>The script text</returns>
		public static string LoadScriptFromXml(Uri path, string scriptName)
		{
			if(path==null) throw new ArgumentNullException("path");
			if(scriptName==null) throw new ArgumentNullException("scriptName");
			
			XmlDocument doc=StorageManager.Get(path).ReadXmlDocument();
			
			string xpath=string.Format("Script[@name='{0}']",scriptName);
			XmlNode node=doc.DocumentElement.SelectSingleNode(xpath);
			if(node==null) throw new ArrowException("script not found: "+scriptName);
			
			return node.InnerText.Trim();
		}
		
		/// <summary>
		/// Creates a uri to a script that is embedded within the same assembly as the model type.
		/// This is useful when you embed the sql to be used by the concrete model type in the same
		/// assembly as the type.
		/// 
		/// You need to remember to prefix the path with the default namespace as any embedded resource has
		/// this prefixed to its name by the compiler
		/// </summary>
		/// <param name="partialPath">The partial path to the script. If it does not end with ".sql" then this will be added</param>
		/// <returns>A uri for the resource.</returns>
		protected static Uri EmbeddedScriptUri(string partialPath)
		{
			if(partialPath==null) throw new ArgumentNullException("partialPath");
			
			Assembly assembly=typeof(T).Assembly;
			AssemblyName assemblyName=new AssemblyName(assembly.FullName);	
			
			string path=string.Format("res://{0}/{1}",assemblyName.Name,partialPath);
			return new Uri(path);
		}
		
		/// <summary>
		/// Used to select single row/column results (such as select count(*) from X)
		/// where we don't want to have to create a type specially for the result
		/// </summary>
		/// <typeparam name="CV">The type of the value being selected</typeparam>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="scriptUri">The location of the script.</param>
		/// <returns>The value of the select</returns>
		protected static CV SelectSingleValue<CV>(DatabaseContext context, Uri scriptUri)
		{
			string sqlScript=StorageManager.Get(scriptUri).ReadString();
			return SelectSingleValue<CV>(context,sqlScript);
		}
		
		/// <summary>
		/// Used to select single row/column results (such as select count(*) from X)
		/// where we don't want to have to create a type specially for the result
		/// </summary>
		/// <typeparam name="CV">The type of the value being selected</typeparam>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="sqlScript">The SQL to run</param>
		/// <returns>The value of the select</returns>
		protected static CV SelectSingleValue<CV>(DatabaseContext context, string sqlScript)
		{
			if(context==null) throw new ArgumentNullException("context");
			if(sqlScript==null) throw new ArgumentNullException("sqlScript");
			
			using(IDbCommand command=context.CreateCommand())
			{
				command.CommandText=sqlScript;
				return SelectSingleValue<CV>(command);				
			}
		}
		
		/// <summary>
		/// Used to select single row/column results (such as select count(*) from X)
		/// where we don't want to have to create a type specially for the result
		/// </summary>
		/// <typeparam name="CV">The type of the value being selecte</typeparam>
		/// <param name="command">The command object that will provide the result</param>
		/// <returns>The value of the select</returns>
		protected static CV SelectSingleValue<CV>(IDbCommand command)
		{
			if(command==null) throw new ArgumentNullException("command");
			
			using(IDataReader reader=command.ExecuteReader())
			{
				// There should only be 1 row and 1 column
				if(reader.Read()==false) throw new ArrowException("no rows returned");
				if(reader.FieldCount!=1) throw new ArrowException("only expected 1 column");
				
				object fieldValue=reader.GetValue(0);
				if(DBNull.Value.Equals(fieldValue)) return default(CV);
				
				if(fieldValue.GetType()==typeof(CV)) return (CV)fieldValue;
				
				Type fieldType=reader.GetFieldType(0);
				
				// We'll need to do a conversion
				TypeConverter converter=TypeDescriptor.GetConverter(typeof(CV));
				if(converter!=null && converter.CanConvertFrom(fieldType)) return (CV)converter.ConvertFrom(fieldValue);
				
				converter=TypeDescriptor.GetConverter(fieldType);
				if(converter!=null && converter.CanConvertTo(typeof(CV))) return (CV)converter.ConvertTo(fieldValue,typeof(CV));
				
				return (CV)Convert.ChangeType(fieldValue,typeof(CV));
			}
		}
		
		/// <summary>
		/// Creates a list of T by running a SQL script against a connection.
		/// </summary>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="scriptUri">The location of the script.</param>
		/// <returns>A list</returns>
		protected static List<T> CreateList(DatabaseContext context, Uri scriptUri)
		{
			string sqlScript=StorageManager.Get(scriptUri).ReadString();
			return CreateList(context,sqlScript);
		}
		
		/// <summary>
		/// Creates a list of T by running a SQL script against a connection.
		/// </summary>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="sqlScript">The SQL to run</param>
		/// <returns>A list</returns>
		protected static List<T> CreateList(DatabaseContext context, string sqlScript)
		{
			List<T> list=new List<T>(EnumerateOver(context,sqlScript));
			return list;
		}
		
		/// <summary>
		/// Enumerates rows of T by running a SQL script against a connection.
		/// Instances are created on demand and the caller must ensure that all database classes are valid whilst enumerating.
		/// </summary>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="scriptUri">The location of the script.</param>
		/// <returns>An enumerator</returns>
		protected static IEnumerable<T> EnumerateOver(DatabaseContext context, Uri scriptUri)
		{
			string sqlScript=StorageManager.Get(scriptUri).ReadString();
			return EnumerateOver(context,sqlScript);
		}
	
		/// <summary>
		/// Enumerates rows of T by running a SQL script against a connection.
		/// Instances are created on demand and the caller must ensure that all database classes are valid whilst enumerating.
		/// </summary>
		/// <param name="context">The context to use to connect to the database</param>
		/// <param name="sqlScript">The SQL to run</param>
		/// <returns>An enumerator</returns>
		protected static IEnumerable<T> EnumerateOver(DatabaseContext context, string sqlScript)
		{
			if(context==null) throw new ArgumentNullException("context");
			if(sqlScript==null) throw new ArgumentNullException("sqlScript");
			
			using(IDbCommand command=context.CreateCommand())
			{
				command.CommandText=sqlScript;
				
				using(IDataReader reader=command.ExecuteReader())
				{
					foreach(T value in EnumerateOver(reader))
					{
						yield return value;
					}
				}
			}	
		}
		
		/// <summary>
		/// Creates a list of T by executing a command
		/// </summary>
		/// <param name="command">The command to execute. The method does not take ownership of the command</param>
		/// <returns>A list</returns>
		protected static List<T> CreateList(IDbCommand command)
		{
			List<T> list=new List<T>(EnumerateOver(command));
			return list;
		}
		
		/// <summary>
		/// Enumerates rows of T by execution a command.
		/// Instances are created on demand and the caller must ensure that all database classes are valid whilst enumerating.
		/// </summary>
		/// <param name="command">The command to execute. The method does not take ownership of the command</param>
		/// <returns>An enumerator</returns>
		protected static IEnumerable<T> EnumerateOver(IDbCommand command)
		{
			if(command==null) throw new ArgumentNullException("command");
			
			using(IDataReader reader=command.ExecuteReader())
			{
				foreach(T value in EnumerateOver(reader))
				{
					yield return value;
				}
			}
		}
		
		/// <summary>
		/// Returns a list of T by extracting fields and mapping them to properties.
		/// </summary>
		/// <param name="reader">The reader containing the data to map</param>
		/// <returns>A list</returns>
		protected static List<T> CreateList(IDataReader reader)
		{
			List<T> list=new List<T>(EnumerateOver(reader));
			return list;
		}
	
		/// <summary>
		/// Enumerates rows of T by extracting fields and mapping them to properties.
		/// Any underscores in a field name are removed prior to mapping
		/// Instances are created on demand and the caller must ensure that all database classes are valid whilst enumerating.
		/// </summary>
		/// <param name="reader">The reader containing the data to map</param>
		/// <returns>An enumerator</returns>
		protected static IEnumerable<T> EnumerateOver(IDataReader reader)
		{
			if(reader==null) throw new ArgumentNullException("reader");
		
			var rowMetaData=ExtractRowMetaData(reader);
			
			while(reader.Read())
			{
				T row=new T();
			
				for(int i=0; i<rowMetaData.Count; i++)
				{
					RowMetaData metaData=rowMetaData[i];
					
					// Handle database nulls
					object fieldValue=reader.GetValue(metaData.FieldIndex);
					if(DBNull.Value.Equals(fieldValue)) fieldValue=null;
					
					// Convert the value if need be
					if(fieldValue!=null && metaData.TypesAreCompatible==false)
					{
						if(metaData.FromTypeConverter!=null) fieldValue=metaData.FromTypeConverter.ConvertFrom(fieldValue);
						else if(metaData.ToTypeConverter!=null) fieldValue=metaData.ToTypeConverter.ConvertTo(fieldValue,metaData.PropertyType);
						else if(metaData.DoStandardConvert) fieldValue=Convert.ChangeType(fieldValue,metaData.PropertyType);
					}
					
					metaData.SetMethod.Invoke(row,new object[]{fieldValue});
				}
				
				yield return row;
			}			
		}
		
		/// <summary>
		/// Extract the metadata needed to map a row to properties within T
		/// </summary>
		/// <param name="reader">The reader that holds the rows to process</param>
		/// <returns>Metadata about the mapping</returns>
		private static List<RowMetaData> ExtractRowMetaData(IDataReader reader)
		{
			var metaData=new List<RowMetaData>();
		
			for(int i=0; i<reader.FieldCount; i++)
			{
				string originalFieldName=reader.GetName(i);
				string fieldName=originalFieldName.Replace("_","");
				PropertyInfo property=typeof(T).GetProperty(fieldName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.IgnoreCase);
				if(property!=null)
				{
					MethodInfo setMethod=property.GetSetMethod();
					if(setMethod!=null) 
					{
						RowMetaData rowData=new RowMetaData();
					
						rowData.SetMethod=setMethod;
						rowData.PropertyType=property.PropertyType;
						rowData.FieldIndex=i;
						
						// If the field and the property aren't the same type then we'll need to do a conversion
						Type fieldType=reader.GetFieldType(i);
						if(property.PropertyType==fieldType)
						{
							rowData.TypesAreCompatible=true;
						}
						else
						{
							TypeConverter converter=TypeDescriptor.GetConverter(property.PropertyType);
							if(converter!=null && converter.CanConvertFrom(fieldType)) 
							{
								rowData.FromTypeConverter=converter;
							}
							else
							{
								converter=TypeDescriptor.GetConverter(fieldType);
								if(converter!=null && converter.CanConvertTo(property.PropertyType))
								{
									rowData.ToTypeConverter=converter;
								}
								else
								{								
									rowData.DoStandardConvert=true;
								}
							}
						}
						
						metaData.Add(rowData);	
					}
				}
			}
			
			return metaData;
		}
		
		class RowMetaData
		{
			public int FieldIndex{get;set;}
			public MethodInfo SetMethod{get;set;}
			public bool TypesAreCompatible{get;set;}
			public TypeConverter FromTypeConverter{get;set;}
			public TypeConverter ToTypeConverter{get;set;}
			public Type PropertyType{get;set;}
			public bool DoStandardConvert{get;set;}
		}
	}
}
