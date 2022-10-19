using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Diagnostics.CodeAnalysis;

using Arrow.Scripting;
using Arrow.Compiler;
using Arrow.Dynamic;
using Arrow.Reflection;


namespace Arrow.Scripting.Wire
{
	partial class Parser
	{
		private readonly TypeCache m_TypeCache=new TypeCache(CaseMode.Insensitive);

		protected internal static readonly Expression Null=Expression.Constant(null);

		private readonly ParseContext m_ParseContext;
		private ExpressionFactory m_ExpressionFactory = default!;
		
		protected readonly ITokenizer m_Tokenizer;

		public Parser(ITokenizer tokenizer, ParseContext parseContext)
		{
			m_ParseContext=parseContext;
			m_Tokenizer=tokenizer;

			m_TypeCache.RegisterCLS();
		}

		protected ExpressionFactory ExpressionFactory
		{
			get{return m_ExpressionFactory;}
			set{m_ExpressionFactory=value;}			
		}

		protected internal void RequireType<T>(Expression expression, string operation)
		{
			if(expression.Type!=typeof(T))
			{
				string message=string.Format("operator '{0}' must be {1}",operation,typeof(T).ToString());
				throw MakeException(message);
			}
		}

		protected Type? ResolveType(string name)
		{
			if(m_TypeCache.TryGetType(name,out var cachedType)) return cachedType;

			List<Type> types=new List<Type>();

			if(name.Contains("."))
			{
				// It's a dotted name, so go through the references looking for a match
				foreach(var assembly in m_ParseContext.References)	
				{
					var type=assembly.GetType(name,false,true);
					if(type!=null) types.Add(type);
				}
			}
			else
			{
				// It's not a fully qualified type, so expand
				// it with the "usings" and look in each assembly
				foreach(string use in m_ParseContext.Usings)			
				{
					string fullname=string.Format("{0}.{1}",use,name);

					foreach(var assembly in m_ParseContext.References)	
					{
						var type=assembly.GetType(fullname,false,true);
						if(type!=null) types.Add(type);
					}
				}
			}

			if(types.Count==0) return null;
			if(types.Count!=1) throw MakeException("ambigious type: "+name);

			cachedType=types[0];
			m_TypeCache.Add(name,cachedType);

			return cachedType;
		}

		/// <summary>
		/// Normalizes two expression prior to a binary operation.
		/// This typically involves checking to see if a type conversion
		/// is required, and if so adjusting the expressions
		/// </summary>
		/// <param name="lhs">The left side of the binary operation</param>
		/// <param name="rhs">The right side of the binary operation</param>
		protected void NormalizeBinaryExpression(ref Expression lhs, ref Expression rhs)
		{
			bool normalized=TypeCoercion.NormalizeBinaryExpression(ref lhs, ref rhs);
			if(normalized==false) throw MakeException("could not find a common type");
		}

		private void NormalizeNumbericBinaryExpression(ref Expression lhs, ref Expression rhs)
		{
			TypeCoercion.NormalizeNumbericBinaryExpression(ref lhs, ref rhs);
		}

		protected internal Exception MakeException(string message)
		{
			var exception=new ParserException(message);
			exception.LineNumber=m_Tokenizer.LineNumber;
			exception.Filename=m_Tokenizer.Filename;

			return exception;
		}
	}
}
