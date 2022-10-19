using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Describes how to call a strongly typed method
	/// </summary>
	public class CallData : ICallData
	{
		public readonly MethodInfo m_Method;
		public readonly IList<Expression> m_Arguments;
		public readonly int m_Cost;

		internal CallData(MethodInfo method, IList<Expression> arguments, int cost)
		{
			m_Method=method;
			m_Arguments=arguments;
			m_Cost=cost;
		}

		public MethodCallExpression Make(Type type, Expression? instance)
		{
			if(instance!=null)
			{
				return Expression.Call(instance,m_Method,m_Arguments);
			}
			else
			{
				return Expression.Call(m_Method,m_Arguments);
			}
		}

		/// <summary>
		/// The cost of the call
		/// </summary>
		public int Cost
		{
			get{return m_Cost;}
		}
	}
}
