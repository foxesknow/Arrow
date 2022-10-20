using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Collections;
using Arrow.Compiler;
using Arrow.Dynamic;
using Arrow.Reflection;
using Arrow.Scripting;

namespace Arrow.Scripting.Wire
{
	abstract partial class Parser
	{
		protected Expression GenerateExpression()
		{
			var expression=ParseExpression();

			if(m_Tokenizer.Current.ID!=TokenID.None) throw MakeException("error parsing data");
			return expression;
		}

		private Expression ParseExpression()
		{
			var expression=Ternary();
			return expression;
		}

		private Expression Ternary()
		{
			Expression expression=NullCoalesce();

			while(m_Tokenizer.TryAccept(TokenID.Question))
			{
				// Call ourself to associate to the right
				Expression ifTrue=Ternary();
				m_Tokenizer.Expect(TokenID.Colon);
				Expression ifFalse=Ternary();

				expression=m_ExpressionFactory.Ternary(expression,ifTrue,ifFalse);
			}

			return expression;
		}

		private Expression NullCoalesce()
		{
			Expression expression=LogicalOr();

			while(m_Tokenizer.TryAccept(TokenID.NullCoalesce))
			{
				// NOTE: We call ourself to be right associative
				Expression rhs=NullCoalesce();
				expression=m_ExpressionFactory.NullCoalesce(expression,rhs);
			}

			return expression;
		}

		private Expression LogicalOr()
		{
			Expression expression=LogicalAnd();

			while(m_Tokenizer.TryAccept(TokenID.LogicalOr))
			{				
				Expression rhs=LogicalAnd();
				expression=m_ExpressionFactory.LogicalOr(expression,rhs);
			}

			return expression;
		}

		/// <summary>
		/// Handles logical (boolean) and
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private Expression LogicalAnd()
		{
			Expression expression=Equality();

			while(m_Tokenizer.TryAccept(TokenID.LogicalAnd))
			{
				Expression rhs=Equality();
				expression=m_ExpressionFactory.LogicalAnd(expression,rhs);
			}

			return expression;
		}

		private Expression Equality()
		{
			var expression=Relational();

			while(m_Tokenizer.TryAcceptOneOf(out var token,TokenID.EqualTo,TokenID.NotEquals,TokenID.EqualToNoCase,TokenID.NotEqualsNoCase,TokenID.RegexEquals))
			{
				var rhs=Relational();

				switch(token.ID)
				{
					case TokenID.EqualTo:
						expression=m_ExpressionFactory.Equal(CaseMode.Sensitive,expression,rhs);
						break;

					case TokenID.EqualToNoCase:
						expression=m_ExpressionFactory.Equal(CaseMode.Insensitive,expression,rhs);
						break;

					case TokenID.NotEquals:
						expression=m_ExpressionFactory.NotEqual(CaseMode.Sensitive,expression,rhs);
						break;

					case TokenID.NotEqualsNoCase:
						expression=m_ExpressionFactory.NotEqual(CaseMode.Insensitive,expression,rhs);
						break;

					case TokenID.RegexEquals:
						expression=m_ExpressionFactory.RegexEqual(expression,rhs);
						break;

					default:
						throw MakeException("unexpected equality-op");
				}
			}

			return expression;
		}

		private Expression Relational()
		{
			var expression=InBetween();

			while(m_Tokenizer.TryAcceptOneOf(out var token,TokenID.GreaterThan,TokenID.GreaterThanOrEqual,TokenID.LessThan,TokenID.LessThanOrEqual))
			{
				var rhs=InBetween();

				switch(token.ID)
				{
					case TokenID.GreaterThan:
						expression=m_ExpressionFactory.GreaterThan(expression,rhs);
						break;

					case TokenID.GreaterThanOrEqual:
						expression=m_ExpressionFactory.GreaterThanOrEqual(expression,rhs);
						break;

					case TokenID.LessThan:
						expression=m_ExpressionFactory.LessThan(expression,rhs);
						break;

					case TokenID.LessThanOrEqual:
						expression=m_ExpressionFactory.LessThanOrEqual(expression,rhs);
						break;

					default:
						throw MakeException("unexpected relational-op");
				}
			}

			return expression;
		}

		private Expression InBetween()
		{
			Expression expression=AddSubtract();

			while(m_Tokenizer.TryAcceptOneOf(out var token,TokenID.In,TokenID.InNoCase,TokenID.Between,TokenID.Like,TokenID.LikeNoCase))
			{
				switch(token.ID)
				{
					case TokenID.In:
					case TokenID.InNoCase:
					{
						CaseMode mode=(token.ID==TokenID.In ? CaseMode.Sensitive : CaseMode.Insensitive);
						var items=ExtractMethodArguments();
						expression=GenerateIn(mode,expression,items);
						break;
					}

					case TokenID.Between:
					{
						Expression first=AddSubtract();
						m_Tokenizer.Expect(TokenID.LogicalAnd);
						Expression second=AddSubtract();
						expression=GenerateBetween(expression,first,second);
						break;
					}

					case TokenID.Like:
					case TokenID.LikeNoCase:
					{
						CaseMode mode=(token.ID==TokenID.Like ? CaseMode.Sensitive : CaseMode.Insensitive);
						Expression pattern=AddSubtract();
						expression=m_ExpressionFactory.Like(mode,expression,pattern);
						break;
					}

					default:
						throw MakeException("unexpected in-like-between-op");
				}
			}

			return expression;
		}

		private Expression AddSubtract()
		{
			var expression=MultiplyDivide();

			while(m_Tokenizer.TryAcceptOneOf(out var token,TokenID.Add,TokenID.Subtract))
			{
				var rhs=MultiplyDivide();
				
				switch(token.ID)
				{
					case TokenID.Add:
						expression=m_ExpressionFactory.Add(expression,rhs);
						break;

					case TokenID.Subtract:
						expression=m_ExpressionFactory.Subtract(expression,rhs);
						break;

					default:
						throw MakeException("unexpected add-op");

				}
			}

			return expression;
		}

		private Expression MultiplyDivide()
		{
			var expression=Unary();

			while(m_Tokenizer.TryAcceptOneOf(out var token,TokenID.Multiply,TokenID.Divide,TokenID.Modulo))
			{
				var rhs=Unary();

				switch(token.ID)
				{
					case TokenID.Multiply:
						expression=m_ExpressionFactory.Multiply(expression,rhs);
						break;

					case TokenID.Divide:
						expression=m_ExpressionFactory.Divide(expression,rhs);
						break;

					case TokenID.Modulo:
						expression=m_ExpressionFactory.Modulo(expression,rhs);
						break;

					default:
						throw MakeException("unexpected mul-op");
				}
			}

			return expression;
		}

		private Expression Unary()
		{
			Expression? expression=null;

			switch(m_Tokenizer.Current.ID)
			{
				case TokenID.Not:
					m_Tokenizer.Accept();
					expression=Factor();
					expression=m_ExpressionFactory.Not(expression);
					break;

				case TokenID.Add:
					m_Tokenizer.Accept();
					expression=Factor();
					break;

				case TokenID.Subtract:
					m_Tokenizer.Accept();
					expression=Factor();
					expression=m_ExpressionFactory.UnaryMinus(expression);
					break;

				default:
					expression=Factor();
					break;
			}

			return expression;
		}

		private Expression Factor()
		{
			Expression? expression=null;

			Token? tempToken=null;
			Token token=m_Tokenizer.Current;

			if(m_Tokenizer.TryAccept(TokenID.Symbol))
			{
				expression=Symbol(token.Data!);
			}
			else if(m_Tokenizer.TryAccept(TokenID.LeftParen))
			{
				expression=ParseExpression();
				m_Tokenizer.Expect(TokenID.RightParen);
			}
			else if(m_Tokenizer.TryAccept(TokenID.Number))
			{
				expression=Expression.Constant(token.ConvertedData);
			}
			else if(m_Tokenizer.TryAccept(TokenID.String))
			{
				expression=Expression.Constant(token.Data);
			}
			else if(m_Tokenizer.TryAccept(TokenID.Char))
			{
				expression=Expression.Constant(token.ConvertedData);
			}
			else if(m_Tokenizer.TryAccept(TokenID.True))
			{
				expression=ExpressionConstants.True;
			}
			else if(m_Tokenizer.TryAccept(TokenID.False))
			{
				expression=ExpressionConstants.False;
			}
			else if(m_Tokenizer.TryAccept(TokenID.Null))
			{
				expression=Parser.Null;
			}
			else if(m_Tokenizer.TryAccept(TokenID.IIf))
			{
				expression=IIf();
			}
			else if(m_Tokenizer.TryAccept(TokenID.Select))
			{
				expression=Select();
			}
			else if(m_Tokenizer.TryAcceptOneOf(out tempToken,TokenID.Cast,TokenID.Is,TokenID.As))
			{
				expression=Cast(tempToken.ID);
			}
			else
			{
				throw MakeException("Unexpected factor: "+token.Data);
			}

			expression=Primary(expression);

			return expression;
		}

		private Expression Primary(Expression expression)
		{
			// Now that we've resolved our factor to an expression
			// we need to see if the user wants to do anything with
			// it, such as calling a property or method
			while(m_Tokenizer.CurrentOneOf(TokenID.MemberAccess,TokenID.ConditionalMemberAccess,TokenID.LeftSquare))
			{
				switch(m_Tokenizer.Current.ID)
				{
					case TokenID.MemberAccess:
						expression=InstanceAccess(expression,TokenID.MemberAccess);
						break;

					case TokenID.ConditionalMemberAccess:
						expression=ConditionalInstanceAccess(expression);
						break;

					case TokenID.LeftSquare:
						expression=ArrayAccess(expression);
						break;

					default:
						throw MakeException("Unhandled primary-op: "+m_Tokenizer.Current);
				}
			}

			return expression;
		}

		private Expression IIf()
		{
			m_Tokenizer.Expect(TokenID.LeftParen);
			Expression condition=ParseExpression();

			m_Tokenizer.Expect(TokenID.Comma);
			Expression ifTrue=ParseExpression();

			m_Tokenizer.Expect(TokenID.Comma);
			Expression ifFalse=ParseExpression();
			
			m_Tokenizer.Expect(TokenID.RightParen);

			return m_ExpressionFactory.Ternary(condition,ifTrue,ifFalse);
		}

		private Expression Select()
		{
			m_Tokenizer.Expect(TokenID.LeftParen);
			var selectValue=ParseExpression();

			// We need to make sure we only evaluate the value once, so store it in a variable
			var localTarget=Expression.Variable(selectValue.Type);
			var assignment=Expression.Assign(localTarget,selectValue);

			var caseExpressions=new List<Tuple<Expression,Expression>>();
			Expression? defaultCondition=null;

			while(m_Tokenizer.TryAccept(TokenID.Comma))
			{
				if(m_Tokenizer.TryAccept(TokenID.Default))
				{
					if(defaultCondition!=null) throw MakeException("multiple default conditions detected in select expression");
					
					m_Tokenizer.Expect(TokenID.LeadsTo);
					defaultCondition=ParseExpression();
				}
				else
				{
					var caseExpression=ParseExpression();
					m_Tokenizer.Expect(TokenID.LeadsTo);
					var valueExpression=ParseExpression();

					caseExpressions.Add(Tuple.Create(caseExpression,valueExpression));
				}
			}

			m_Tokenizer.Expect(TokenID.RightParen);
			if(defaultCondition==null) throw MakeException("you must specify a  default condition in a select expression");

			Expression? selectEvaluation=null;

			if(caseExpressions.Count==0)
			{
				// Easy, there's just a default, so that's the outcome.
				// Strictly speaking we don't need to evaluate the "select value" as we're always going to
				// return the default value. However, to be consistent with the case where there are
				// conditions we'll do the evaluation
				selectEvaluation=defaultCondition;
			}
			else
			{
				// We need to create a series of if/else statements. We can do this with the ternary expression
				selectEvaluation=defaultCondition;

				for(int i=caseExpressions.Count-1; i>=0; i--)
				{
					var @case=caseExpressions[i];
					var value=@case.Item1;
					var result=@case.Item2;

					var condition=m_ExpressionFactory.Equal(CaseMode.Sensitive,localTarget,value);
					selectEvaluation=m_ExpressionFactory.Ternary(condition,result,selectEvaluation);
				}
			}

			var block=Expression.Block
			(
				Sequence.Single(localTarget),
				assignment,
				selectEvaluation	
			);

			return block;
		}

		protected Expression Cast(int id)
		{
			m_Tokenizer.Expect(TokenID.LessThan);
			
			string typeName=ExtractTypeName();
			var type=ResolveType(typeName);			
			if(type==null) throw MakeException("could not resolve type: "+typeName);
			
			m_Tokenizer.Expect(TokenID.GreaterThan);

			m_Tokenizer.Expect(TokenID.LeftParen);
			Expression whatToCast=ParseExpression();
			m_Tokenizer.Expect(TokenID.RightParen);

			Expression? expression=null;

			if(id==TokenID.Cast)
			{
				expression=m_ExpressionFactory.Cast(whatToCast,type);
			}
			else if(id==TokenID.As)
			{
				if(type.IsValueType==false) throw MakeException("As cannot be used with value type");
				expression=m_ExpressionFactory.As(whatToCast,type);
			}
			else if(id==TokenID.Is)
			{
				expression=m_ExpressionFactory.Is(whatToCast,type);
			}
			else
			{
				throw MakeException("Unsupported cast operation");
			}
			
			return expression;
		}


		private Expression GenerateIn(CaseMode caseMode, Expression target, IList<Expression> items)
		{
			if(items.Count==0) return ExpressionConstants.False;

			// We need to evaluate the target only once
			var localTarget=Expression.Variable(target.Type);
			var assignment=Expression.Assign(localTarget,target);

			Expression? expression=null;

			// We need to reverse the list as we want the expression to
			// expand to target==item1 || (target==item2 || (target==item3))) etc
			foreach(var item in items.Reverse())
			{
				Expression lhs=localTarget;
				Expression rhs=item;

				Expression comparison=m_ExpressionFactory.Equal(caseMode,lhs,rhs);

				if(expression==null)
				{
					expression=comparison;
				}
				else
				{
					expression=m_ExpressionFactory.LogicalOr(comparison,expression);
				}
			}

			Expression block=Expression.Block
			(
				Sequence.Single(localTarget), // The variables in the block
				assignment,
				expression!
			);

			return block;
		}

		private Expression GenerateBetween(Expression item, Expression lhs, Expression rhs)
		{
			// We need to make sure we only evaluate item once
			var localItem=Expression.Variable(item.Type);
			var assignment=Expression.Assign(localItem,item);
			
			Expression firstItem=localItem;
			Expression firstComparison=m_ExpressionFactory.GreaterThanOrEqual(firstItem,lhs);

			Expression secondItem=localItem;
			Expression secondComparison=m_ExpressionFactory.LessThan(secondItem,rhs);

			Expression block=Expression.Block
			(
				Sequence.Single(localItem), // The variables in the block
				assignment,
				m_ExpressionFactory.LogicalAnd(firstComparison,secondComparison)
			);

			return block;
		}

		protected IList<Expression> ExtractMethodArguments()
		{
			return ExtractSequence(TokenID.LeftParen,TokenID.RightParen);
		}

		protected IList<Expression> ExtractArrayIndexes()
		{
			return ExtractSequence(TokenID.LeftSquare,TokenID.RightSquare);
		}

		protected IList<Expression> ExtractSequence(int startID, int stopID)
		{
			m_Tokenizer.Expect(startID);

			var sequence=new List<Expression>();

			if(m_Tokenizer.Current.ID!=stopID)
			{
				while(true)
				{
					var parameter=ParseExpression();
					sequence.Add(parameter);
					
					if(m_Tokenizer.Current.ID==stopID) break;
					m_Tokenizer.Expect(TokenID.Comma);
				}
			}

			m_Tokenizer.Expect(stopID);

			return sequence;
		}

		private string ExtractTypeName()
		{
			StringBuilder name=new StringBuilder();

			var symbol=m_Tokenizer.Expect(TokenID.Symbol);
			name.Append(symbol.Data);

			while(m_Tokenizer.Current.ID==TokenID.MemberAccess)
			{
				m_Tokenizer.NextToken();
				name.Append('.');

				symbol=m_Tokenizer.Expect(TokenID.Symbol);
				name.Append(symbol.Data);
			}

			return name.ToString();
		}

		protected virtual bool AllowConditionMemberAccess
		{
			get{return false;}
		}

		protected virtual Expression ConditionalInstanceAccess(Expression instance)
		{
			if(instance.Type.IsValueType==false)
			{
				// We need to check for null and only make the call if we've got an instance
				Expression nullValue=Parser.Null.ConvertTo<object>();
				Expression instanceAccess=InstanceAccess(instance,TokenID.ConditionalMemberAccess);
				Expression defaultValue=Expression.Default(instanceAccess.Type);
				Expression condition=Expression.Equal(instance.ConvertTo<object>(),nullValue);

				return Expression.Condition(condition,defaultValue,instanceAccess);
			}
			else
			{			
				return InstanceAccess(instance,TokenID.ConditionalMemberAccess);
			}
		}

		protected virtual Expression InstanceAccess(Expression instance, int accessToken)
		{
			m_Tokenizer.Expect(accessToken);

			// Grab the name of the thing we're after
			string? name=m_Tokenizer.Current.Data;
			m_Tokenizer.Expect(TokenID.Symbol);

			Expression? access=null;

			if(m_Tokenizer.Current.ID==TokenID.LeftParen)
			{
				// It's a method call
				var parameters=ExtractMethodArguments();
				access=m_ExpressionFactory.InstanceCall(instance,name!,parameters);
			}
			else if(m_Tokenizer.Current.ID==TokenID.LeftSquare)
			{
				var arrayBounds=ExtractSequence(TokenID.LeftSquare,TokenID.RightSquare);
				access=m_ExpressionFactory.InstancePropertyOrFieldWithArgs(instance,name!,arrayBounds);
			}
			else
			{
				access=m_ExpressionFactory.InstancePropertyOrField(instance,name!);
			}

			return access;
		}

		protected virtual Expression StaticAccess(Type type)
		{
			m_Tokenizer.Expect(TokenID.MemberAccess);
			
			// Grab the name of the thing we're after
			string? name=m_Tokenizer.Current.Data;
			m_Tokenizer.Expect(TokenID.Symbol);

			Expression? access=null;

			if(m_Tokenizer.Current.ID==TokenID.LeftParen)
			{
				// It's a method call
				var parameters=ExtractMethodArguments();
				access=m_ExpressionFactory.StaticCall(type,name!,parameters);
			}
			else if(m_Tokenizer.Current.ID==TokenID.LeftSquare)
			{
				var bounds=ExtractArrayIndexes();
				access=m_ExpressionFactory.StaticPropertyOrFieldWithArgs(type,name!,bounds);
			}
			else
			{			
				// It's a property of a field
				access=m_ExpressionFactory.StaticPropertyOrField(type,name!);
			}

			return access;
		}

		protected virtual Expression ArrayAccess(Expression instance)
		{
			var bounds=ExtractArrayIndexes();

			Expression arrayAccess=m_ExpressionFactory.ArrayAccess(instance,bounds);
			return arrayAccess;
		}

		protected abstract Expression Symbol(string symbolName);
	}
}
