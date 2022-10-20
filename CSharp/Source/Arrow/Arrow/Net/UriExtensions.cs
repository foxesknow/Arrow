using Arrow.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
    public static class UriExtensions
    {
        /// <summary>
        /// Adds a parameters to a query string
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Uri AddParameter(this Uri uri, string name, string? value)
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name", nameof(name));

            var parameters = uri.QueryParameters()
                                .AddToEnd((name, value));

            var builder = new UriBuilder(uri);
            builder.Query = FlattenQueryParameters(parameters);

            return builder.Uri;
        }

        /// <summary>
        /// Adds or updates a parameter on a query string
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Uri AddOrUpdateParameter(this Uri uri, string name, string? value)
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name", nameof(name));

            var parameters = uri.QueryParameters().ToList();
            var comparer = StringComparer.OrdinalIgnoreCase;

            var found = false;
            for(int i = 0; i < parameters.Count; i++)
            {
                var pair = parameters[i];
                if(comparer.Equals(pair.Name, name))
                {
                    // Found it!
                    found = true;
                    parameters[i] = (pair.Name, value);
                }
            }

            if(found == false)
            {   
                parameters.Add((name, value));
            }

            var builder = new UriBuilder(uri);
            builder.Query = FlattenQueryParameters(parameters);

            return builder.Uri;
        }

        /// <summary>
        /// Removes all parameters with the given name
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Uri RemoveParameter(this Uri uri, string name)
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name", nameof(name));

            var comparer = StringComparer.OrdinalIgnoreCase;
            
            // Get everything other than the parameter
            var parameters = uri.QueryParameters()
                                .Where(p => comparer.Equals(name, p.Name) == false);
         
            var builder = new UriBuilder(uri);
            builder.Query = FlattenQueryParameters(parameters);

            return builder.Uri;
        }

        /// <summary>
        /// Returns the parameters in a query string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<(string Name, string? Value)> QueryParameters(this Uri uri)
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));

            return QueryParameters(uri.Query);
        }

        /// <summary>
        /// Returns the parameters in a query string
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<(string Name, string? Value)> QueryParameters(string queryString)
        {
            if(queryString is null) throw new ArgumentNullException(nameof(queryString));

            return Execute(queryString);

            static IEnumerable<(string Name, string? Value)> Execute(string queryString)
            {
                // Normalize the query, jst in case
                if(queryString.StartsWith("?")) queryString = queryString.Substring(1);

                var queries = queryString.Split(new[]{'&'}, StringSplitOptions.RemoveEmptyEntries);
                foreach(var query in queries)
                {
                    var parts = query.Split(new char[]{'='}, 2);
                    if(parts.Length == 0) continue;

                    string name = Uri.UnescapeDataString(parts[0]);
                    if(parts.Length == 2)
                    {
                        var value = Uri.UnescapeDataString(parts[1]);
                        yield return (name, value);
                    }
                    else
                    {
                        yield return (name, null);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all query data from a uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Uri ClearQuery(this Uri uri)
        {
            if(uri is null) throw new ArgumentNullException(nameof(uri));

            var builder = new UriBuilder(uri);
            builder.Query = null;

            return builder.Uri;
        }

        /// <summary>
        /// Flattens query parameters into a uri query string
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string FlattenQueryParameters(IEnumerable<(string Name, string? Value)> queryParameters)
        {
            if(queryParameters is null) throw new ArgumentNullException(nameof(queryParameters));

            return string.Join("&", queryParameters.Select(q => Encode(q.Name, q.Value)));

            static string Encode(string name, string? value)
            {
                if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid parameter name");

                if(value is null)
                {
                    return Uri.EscapeDataString(name);
                }
                else
                {
                    return $"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}";
                }
            }
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

        /// <summary>
		/// Parses the uri query into a dictionary
		/// </summary>
		/// <param name="uri">The uri to parse</param>
		/// <returns>A dictionary containing the individual queries</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public static Dictionary<string,string?> ParseQuery(this Uri uri)
		{
			if(uri is null) throw new ArgumentNullException("uri");
			return ParseQuery(uri.Query);
		}
		
		/// <summary>
		/// Parses the uri query into a dictionary
		/// </summary>
		/// <param name="queryString">The query to parse</param>
		/// <returns>A dictionary containing the individual queries</returns>
		/// <exception cref="System.ArgumentNullException">queryString is null</exception>
		private static Dictionary<string,string?> ParseQuery(this string queryString)
		{
			if(queryString is null) throw new ArgumentNullException("queryString");
			
			var values = new Dictionary<string,string?>();
			
			// Normalize the query, just in case
			if(queryString.StartsWith("?")) queryString = queryString.Substring(1);
			
			var queries = queryString.Split(new char[]{'&'}, StringSplitOptions.RemoveEmptyEntries);
			foreach(string query in queries)
			{
				var parts = query.Split(new char[]{'='} , 2);
				if(parts.Length == 0) continue;
				
				string name = Uri.UnescapeDataString(parts[0]);
				string? value = null;
				if(parts.Length == 2) value = Uri.UnescapeDataString(parts[1]);
				
				values[name]=value;
			}
			
			return values;
		}
    }
}
