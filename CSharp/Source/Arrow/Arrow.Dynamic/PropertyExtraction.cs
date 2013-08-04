using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Reflection;

namespace Arrow.Dynamic
{
	public static class PropertyExtraction
	{
		/// <summary>
		/// Returns a function that can be used to extract property values
		/// </summary>
		/// <typeparam name="T">The type to return the extractor for</typeparam>
		/// <returns>An extractor for the specified type</returns>
		public static Action<T,Action<string,object>> Extractor<T>()
		{
			return Generator<T>.Instance;
		}

		/// <summary>
		/// Returns a dictionary mapping property names to their current values
		/// </summary>
		/// <typeparam name="T">The type to extract the values from</typeparam>
		/// <param name="item">The item to extract the values from</param>
		/// <returns>A dictionary containing properties and their values</returns>
		public static IDictionary<string,object> Values<T>(T item)
		{
			var extractor=Extractor<T>();

			Dictionary<string,object> values=new Dictionary<string,object>();
			extractor(item,(name,value)=>values.Add(name,value));

			return values;
		}

		static class Generator<T>
		{
			public static readonly Action<T,Action<string,object>> Instance=Generate();

			private static BindingFlags BindFlags=BindingFlags.Public|BindingFlags.Instance;

			private static Action<T,Action<string,object>> Generate()
			{
				var item=Expression.Parameter(typeof(T));
				var handler=Expression.Parameter(typeof(Action<string,object>));

				List<Expression> expressions=new List<Expression>();

				// If it's a reference style type then make sure we've actually been passed a value
				if(typeof(T).CanBeSetToNull())
				{
					var equalsNull=Expression.Equal(item,Expression.Constant(null));
					var nullCheck=Expression.IfThen(equalsNull,ExpressionEx.Throw<ArgumentNullException>());

					expressions.Add(nullCheck);
				}

				// First grab the properties
				foreach(var property in typeof(T).GetProperties(BindFlags))
				{
					// Ignore unreadable properties
					if(property.CanRead==false) continue;
					
					// Ignore properties that take parameters (this[,,,])
					if(property.GetGetMethod().GetParameters().Length!=0) continue;

					var propertyName=Expression.Constant(property.Name);
					var propertyValue=Expression.Property(item,property).ConvertTo<object>();

					var callHandler=Expression.Invoke(handler,propertyName,propertyValue);
					expressions.Add(callHandler);
				}

				// We'll treat any public instance variables as being
				// intended for property style use
				foreach(var field in typeof(T).GetFields(BindFlags))
				{
					var fieldName=Expression.Constant(field.Name);
					var fieldValue=Expression.Field(item,field);

					var callHandler=Expression.Invoke(handler,fieldName,fieldValue);
					expressions.Add(callHandler);
				}

				expressions.Add(Expression.Default(typeof(void)));

				var body=Expression.Block(expressions);
				var lambda=Expression.Lambda<Action<T,Action<string,object>>>(body,item,handler);

				return lambda.Compile();
			}
		}
	}
}
