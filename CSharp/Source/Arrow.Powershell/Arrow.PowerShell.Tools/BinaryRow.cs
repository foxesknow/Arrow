using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Tools
{
	/// <summary>
	/// Represents a row of binary data
	/// </summary>
	public class BinaryRow
	{
		private readonly long m_Address;
		private readonly byte[] m_Data;
		private readonly int m_BlockSize;

		private string m_AsString;

		public BinaryRow(long address, byte[] data, int blockSize)
		{
			m_Address=address;
			m_Data=data;
			m_BlockSize=blockSize;
		}

		/// <summary>
		/// The address the data came from
		/// </summary>
		public long Address
		{
			get{return m_Address;}
		}

		/// <summary>
		/// The binary data
		/// </summary>
		public byte[] Data
		{
			get{return m_Data;}
		}		

		private string Render()
		{
			var builder=new StringBuilder();

			if(m_Address>=int.MaxValue)
			{
				builder.AppendFormat("{0:x16} ",m_Address);
			}
			else
			{
				builder.AppendFormat("{0:x8} ",m_Address);
			}

			// Write the data in hex
			for(int i=0; i<m_BlockSize; i++)
			{
				if(i<m_Data.Length)
				{
					builder.AppendFormat("{0:x2} ",m_Data[i]);
				}
				else
				{
					builder.AppendFormat("   ");
				}
			}

			builder.Append(" ");

			// And how in printable format
			for(int i=0; i<m_Data.Length; i++)
			{
				byte b=m_Data[i];

				if(b>=32 && b<=127)
				{
					builder.Append((char)b);
				}
				else
				{
					builder.Append('.');
				}
			}

			return builder.ToString();
		}

		public override string ToString()
		{
			if(m_AsString==null)
			{
				m_AsString=Render();
			}

			return m_AsString;
		}
	}
}
