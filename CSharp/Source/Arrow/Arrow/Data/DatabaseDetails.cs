using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Arrow.Data
{
	/// <summary>
	/// Abstraction for databases
	/// </summary>
	public abstract class DatabaseDetails
	{
		/// <summary>
		/// Returns a new connection to the database. The caller has ownership of the object
		/// </summary>
		/// <returns>A database connection</returns>
		public abstract IDbConnection CreateConnection();
		
		/// <summary>
		/// Executes a function outside of a transaction
		/// </summary>
		/// <typeparam name="T">The return type of the function to call</typeparam>
		/// <param name="function">The function to call</param>
		/// <returns>The value returned from the function</returns>
		public T Execute<T>(Func<DatabaseContext,T> function)
		{
			if(function==null) throw new ArgumentNullException("function");
			
			using(IDbConnection connection=CreateConnection())
			{
				connection.Open();
				
				var context=new DatabaseContext(connection,null,this);
				return function(context);
			}
		}
		
		/// <summary>
		/// Executes an action outside of a transaction
		/// </summary>
		/// <param name="action">The action to execute</param>
		public void Execute(Action<DatabaseContext> action)
		{
			if(action==null) throw new ArgumentNullException("action");
			
			using(IDbConnection connection=CreateConnection())
			{
				connection.Open();
				
				var context=new DatabaseContext(connection,null,this);
				action(context);
			}
		}
				
		/// <summary>
		/// Executes a series of steps against a database.
		/// The steps are performed within a transaction
		/// </summary>
		/// <param name="steps">The steps to perform</param>
		public void ExecuteInTransaction(params Action<DatabaseContext>[] steps)
		{
			using(IDbConnection connection=CreateConnection())
			{
				connection.Open();
				
				using(IDbTransaction transaction=connection.BeginTransaction())
				{
					var context=new DatabaseContext(connection,transaction,this);
				
					foreach(var step in steps)
					{
						step(context);
					}
				
					transaction.Commit();
				}
			}
		}
	}
}
