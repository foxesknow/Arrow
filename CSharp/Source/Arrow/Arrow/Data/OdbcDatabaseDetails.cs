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
		/// <summary>
		/// Creates a database connection.
		/// </summary>
		/// <returns>A database connection</returns>
		public override IDbConnection CreateConnection()
		{
			if(this.ConnectionString==null) throw new ArgumentNullException("connectionString");
			
			return new OdbcConnection(this.ConnectionString);
		}
		
		/// <summary>
		/// The connection string to use to connect to an ODBC database
		/// </summary>
		public string? ConnectionString{get; set;}
	}
}
