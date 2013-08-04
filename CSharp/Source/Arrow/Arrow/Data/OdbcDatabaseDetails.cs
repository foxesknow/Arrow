using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace Arrow.Data
{
	/// <summary>
	/// Generic ODBC database connection string class
	/// </summary>
	public class OdbcDatabaseDetails : DatabaseDetails
	{
		private string m_ConnectionString;
	
		/// <summary>
		/// Creates a database connection.
		/// </summary>
		/// <returns>A database connection</returns>
		public override IDbConnection CreateConnection()
		{
			if(m_ConnectionString==null) throw new ArgumentNullException("connectionString");
			
			return new OdbcConnection(m_ConnectionString);
		}
		
		/// <summary>
		/// The connection string to use to connect to an ODBC database
		/// </summary>
		public string ConnectionString
		{
			get{return m_ConnectionString;}
			set{m_ConnectionString=value;}
		}
	}
}
