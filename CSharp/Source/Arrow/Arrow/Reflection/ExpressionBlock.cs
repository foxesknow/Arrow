using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Reflection
{
	/// <summary>
	/// Holds a sequence of expression that are treated as a block
	/// </summary>
	public class ExpressionBlock : IEnumerable<Expression>
	{
		private readonly List<Expression> m_Expressions=new List<Expression>();

		/// <summary>
		/// Adds expressions to the block
		/// </summary>
		/// <param name="expressions"></param>
		public void Add(params Expression[] expressions)
		{
			m_Expressions.AddRange(expressions);
		}

		/// <summary>
		/// Adds expressions to the block
		/// </summary>
		/// <param name="expressions"></param>
		public void Add(IEnumerable<Expression> expressions)
		{
			if(expressions==null) throw new ArgumentNullException("expressions");

			m_Expressions.AddRange(expressions);
		}

		/// <summary>
		/// An enumerator for the expressions in the block
		/// </summary>
		/// <returns>An enumeratoe</returns>
		public IEnumerator<Expression> GetEnumerator()
		{
			return m_Expressions.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
