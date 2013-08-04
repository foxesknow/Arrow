using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Arrow.Data
{
	/// <summary>
	/// SQL Server database connection class
	/// </summary>
	public class SqlServerDatabaseDetails : DatabaseDetails
	{
		private string m_ConnectionString;
		
		/// <summary>
		/// Initializes the instance with no connection string
		/// </summary>
		public SqlServerDatabaseDetails() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="connectionString">A connection string</param>
		public SqlServerDatabaseDetails(string connectionString)
		{
			m_ConnectionString=connectionString;
		}
		
		/// <summary>
		/// The connection string to use
		/// </summary>
		public string ConnectionString
		{
			get{return m_ConnectionString;}
			set{m_ConnectionString=value;}
		}
		
		/// <summary>
		/// Creates a database connection.
		/// </summary>
		/// <returns>A database connection</returns>
		public override IDbConnection CreateConnection()
		{
			return new SqlConnection(m_ConnectionString);
		}
	}
}
