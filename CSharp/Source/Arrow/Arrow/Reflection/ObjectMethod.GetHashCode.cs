using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
	public static partial class ObjectMethod
	{
		/// <summary>
		/// Generates a function to compute a hash code off of the public properties of a type
		/// </summary>
		/// <typeparam name="T">The type to generate the function for</typeparam>
		/// <returns>A function that computes a hash code</returns>
		public static Func<T,int> MakeGetHashCode<T>()
		{
			var properties=typeof(T).GetProperties(PublicInstance);

			var body=new ExpressionBlock();

			var @this=Expression.Parameter(typeof(T));

			// Where we'll store the hash as we calculate it
			var hash=Expression.Variable(typeof(int));

			// How we'll scale up the hash for each property
			var multiplier=Expression.Constant(31);

			// Start with an initial value of 13 for the hash code
			body.Add(Expression.Assign(hash,Expression.Constant(13)));

			foreach(var property in properties)
			{
				if(property.PropertyType.IsValueType)
				{
					// No need to worry about null values
					var value=Expression.Property(@this,property);
					var update=ApplyHash(hash,multiplier,value);

					body.Add(update);
				}
				else
				{
					// The reference may be null, so handle this
					var temp=Expression.Variable(property.PropertyType);
					var assign=Expression.Assign(temp,Expression.Property(@this,property));

					var condition=Expression.Condition
					(
						Expression.Call(ReferenceEqualsMethod,temp,NullExpression),
						Expression.MultiplyAssign(hash,multiplier),
						ApplyHash(hash,multiplier,temp)
					);

					var nestedBlock=Expression.Block(AsSequence(temp),assign,condition);
					body.Add(nestedBlock);
				}
			}

			var block=Expression.Block(AsSequence(hash),body);

			var function=Expression.Lambda<Func<T,int>>(block,@this).Compile();
			return function;
		}

		private static Expression ApplyHash(Expression variable, Expression multiplier, Expression value)
		{
			var method=GetGetHashCode(value.Type);

			var multiplyAdd=Expression.Add
			(
				Expression.Multiply(variable,multiplier),
				Expression.Call(value,method)
			);

			return Expression.Assign(variable,multiplyAdd);
		}

		private static MethodInfo GetGetHashCode(Type type)
		{
			return type.GetMethod("GetHashCode",PublicInstance);
		}
	}
}
