using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Arrow.Data
{
	/// <summary>
	/// Encapsulate database information whilst methods are being called
	/// </summary>
	public class DatabaseContext
	{	
		internal DatabaseContext(IDbConnection connection, IDbTransaction? transaction, DatabaseDetails details)
		{
			this.Connection=connection;
			this.Transaction=transaction;
			this.DatabaseDetails=details;
			
			this.Properties=new Dictionary<string,object>();
		}
		
		/// <summary>
		/// A collection of properties that can be used to store
		/// context specific information
		/// </summary>
		public Dictionary<string,object> Properties{get;private set;}
	
		/// <summary>
		/// The connection to the database
		/// </summary>
		public IDbConnection Connection{get;private set;}
		
		/// <summary>
		/// The transaction in use, if applicable
		/// </summary>
		public IDbTransaction? Transaction{get;private set;}
		
		/// <summary>
		/// The details of the database
		/// </summary>
		public DatabaseDetails DatabaseDetails{get;private set;}
		
		/// <summary>
		/// Creates a command for the connection.
		/// If a transaction is active it will be attached to the command
		/// </summary>
		/// <returns>A command</returns>
		public IDbCommand CreateCommand()
		{
			IDbCommand command=this.Connection.CreateCommand();
			command.Transaction=this.Transaction;
			
			return command;
		}
	}
}
