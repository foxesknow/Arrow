using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Messaging.Support
{
	/// <summary>
	/// Provides a reasonable default implementation of a map message
	/// for use in specific implementations that do not have a concreate
	/// message class, such as 29West
	/// </summary>
	[Serializable]
	public class MapMessage : MessageBase, IMapMessage
	{
		private readonly Dictionary<string,object> m_Map=new Dictionary<string,object>();
	
		/// <summary>
		/// Gets a boolean value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The bool with the specified name</returns>
		public bool GetBool(string name)
		{
			return DoGetValue<bool>(name);
		}

		/// <summary>
		/// Sets a bool value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetBool(string name,bool value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a byte value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte with the specified name</returns>
		public byte GetByte(string name)
		{
			return DoGetValue<byte>(name);
		}

		/// <summary>
		/// Sets a byte value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetByte(string name,byte value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a char value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The char with the specified name</returns>		
		public char GetChar(string name)
		{
			return DoGetValue<char>(name);
		}

		/// <summary>
		/// Sets a char value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetChar(string name,char value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a short value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The short with the specified name</returns>
		public short GetShort(string name)
		{
			return DoGetValue<short>(name);
		}

		/// <summary>
		/// Sets a short value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetShort(string name,short value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a int value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The int with the specified name</returns>
		public int GetInt(string name)
		{
			return DoGetValue<int>(name);
		}

		/// <summary>
		/// Sets a int value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetInt(string name,int value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a long value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The long with the specified name</returns>
		public long GetLong(string name)
		{
			return DoGetValue<long>(name);
		}

		/// <summary>
		/// Sets a long value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetLong(string name,long value)
		{
			DoSetValue(name,value);
		}
		
		/// <summary>
		/// Gets a float value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The float with the specified name</returns>
		public float GetFloat(string name)
		{
			return DoGetValue<float>(name);
		}
		
		/// <summary>
		/// Sets a float value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetFloat(string name, float value)
		{
			DoSetValue(name,value);
		}
		
		/// <summary>
		/// Gets a double value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The double with the specified name</returns>
		public double GetDouble(string name)
		{
			return DoGetValue<double>(name);
		}
		
		/// <summary>
		/// Sets a double value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetDouble(string name, double value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a string value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte with the specified name</returns>
		public string GetString(string name)
		{
			return DoGetValue<string>(name);
		}

		/// <summary>
		/// Sets a string value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetString(string name,string value)
		{
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets an object value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The object with the specified name</returns>
		public object GetObject(string name)
		{
			object value;
			
			if(m_Map.TryGetValue(name,out value))
			{
				if(value is byte[]) value=CloneArray((byte[])value);
			}
			
			return value;
		}

		/// <summary>
		/// Sets a object value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetObject(string name,object value)
		{
			ValidateType(name,value);
			DoSetValue(name,value);
		}

		/// <summary>
		/// Gets a byte array value
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>The byte array with the specified name</returns>
		public byte[] GetBytes(string name)
		{
			object value;
			if(m_Map.TryGetValue(name,out value))
			{
				if(value is byte[])
				{
					return CloneArray((byte[])value);
				}
				else
				{
					throw new ArgumentException("not a byte[] - "+name);
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Sets a byte array value
		/// </summary>
		/// <param name="name">The name of the name</param>
		/// <param name="value">The value</param>
		public void SetBytes(string name,byte[] value)
		{
			var copy=CloneArray(value);
			DoSetValue(name,copy);
		}

		/// <summary>
		/// Returns the names of the values held in the message
		/// </summary>
		public IEnumerable<string> Names
		{
			get{return m_Map.Keys;}
		}

		/// <summary>
		/// Indicates if a value is present
		/// </summary>
		/// <param name="name">The name of the value</param>
		/// <returns>true if the map contains the named value, otherwise false</returns>
		public bool Contains(string name)
		{
			return m_Map.ContainsKey(name);
		}
		
		private T DoGetValue<T>(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			
			object value;
			if(m_Map.TryGetValue(name,out value)==false) throw new ArgumentException("value not present "+name);
			
			if(value is T) return (T)value;
			return (T)Convert.ChangeType(value,typeof(T));
		}
		
		private void DoSetValue(string name, object value)
		{
			if(name==null) throw new ArgumentNullException("name");
			m_Map.Add(name,value);
		}
		
		private byte[] CloneArray(byte[] data)
		{
			if(data==null) return data;
			
			byte[] copy=new byte[data.Length];
			Array.Copy(data,copy,data.Length);
			
			return copy;
		}
		
		private void ValidateType(string name, object value)
		{
			if(value!=null)
			{
				TypeCode typeCode=Convert.GetTypeCode(value);
				
				switch(typeCode)
				{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Char:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
						break;
						
					default:
						if(!(value is byte[])) throw new ArgumentException("Unsupported type for "+name);
						break;
				}
			
			}
		}

	}
}
