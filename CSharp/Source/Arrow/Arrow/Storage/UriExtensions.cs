using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Storage
{
	/// <summary>
	/// Useful Uri extension methods
	/// </summary>
	public static class UriExtensions
	{
		/// <summary>
		/// Parses the uri query into a dictionary
		/// </summary>
		/// <param name="uri">The uri to parse</param>
		/// <returns>A dictionary containing the individual queries</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public static Dictionary<string,string> ParseQuery(this Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			return ParseQuery(uri.Query);
		}
		
		/// <summary>
		/// Parses the uri query into a dictionary
		/// </summary>
		/// <param name="queryString">The query to parse</param>
		/// <returns>A dictionary containing the individual queries</returns>
		/// <exception cref="System.ArgumentNullException">queryString is null</exception>
		private static Dictionary<string,string> ParseQuery(this string queryString)
		{
			if(queryString==null) throw new ArgumentNullException("queryString");
			
			Dictionary<string,string> values=new Dictionary<string,string>();
			
			// Normalize the query, just in case
			if(queryString.StartsWith("?")) queryString=queryString.Substring(1);
			
			string[] queries=queryString.Split(new char[]{'&'},StringSplitOptions.RemoveEmptyEntries);
			foreach(string query in queries)
			{
				string[] parts=query.Split(new char[]{'='},2);
				if(parts.Length==0) continue;
				
				string name=Uri.UnescapeDataString(parts[0]);
				string value="";
				if(parts.Length==2) value=Uri.UnescapeDataString(parts[1]);
				
				values[name]=value;
			}
			
			return values;
		}
		
		/// <summary>
		/// Removes the query from a uri
		/// </summary>
		/// <param name="uri">The uri to remove the query from</param>
		/// <returns>A new uri</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public static Uri StripQuery(this Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			UriBuilder builder=new UriBuilder(uri);
			builder.Query="";
			
			return builder.Uri;
		}
		
		/// <summary>
		/// Removes any username and password information from a uri
		/// </summary>
		/// <param name="uri">The uri to strip</param>
		/// <returns>A uri without logon details</returns>
		public static Uri StripLogonDetails(this Uri uri)
		{
			if(uri==null) throw new ArgumentNullException("uri");
			
			UriBuilder builder=new UriBuilder(uri);
			builder.UserName="";
			builder.Password="";
			
			return builder.Uri;
		}
	}
}
