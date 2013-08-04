using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Arrow.Data
{
	/// <summary>
	/// Database class extension methods
	/// </summary>
	public static class DatabaseExtensions
	{
		/// <summary>
		/// Creates a <b>IDbDataParameter</b> instance
		/// </summary>
		/// <param name="command">The command object to create the parameter against</param>
		/// <param name="type">The database type of the parameter</param>
		/// <param name="name">The name of the parameter in the sql</param>
		/// <param name="value">The parameter value</param>
		/// <returns>The parameter</returns>
		public static IDbDataParameter CreateParameter(this IDbCommand command, DbType type, string name, object value)
		{
			IDbDataParameter parameter=command.CreateParameter();
			
			// Map nulls to the correct database null
			if(value==null) value=DBNull.Value;
			
			parameter.DbType=type;
			parameter.ParameterName=name;			
			parameter.Value=value;
			
			return parameter;
		}
		
		/// <summary>
		/// Creates and adds a parameter to a command
		/// </summary>
		/// <param name="command">The command to add the paremter to</param>
		/// <param name="type">The database type of the parameter</param>
		/// <param name="name">The name of the parameter in the sql</param>
		/// <param name="value">The parameter value</param>
		/// <returns>The parameter</returns>
		public static IDbDataParameter AddParameter(this IDbCommand command, DbType type, string name, object value)
		{
			IDbDataParameter parameter=command.CreateParameter(type,name,value);
			command.Parameters.Add(parameter);
			return parameter;
		}
	}
}
