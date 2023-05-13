using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Arrow.Scripting;
using Arrow.Scripting.Wire;
using Arrow.Scripting.Wire.StaticExpression;
using Arrow.Scripting.Wire.DynamicExpression;

using UnitTests.Arrow.Scripting.Wire.TestClasses;

using NUnit.Framework;

namespace UnitTests.Arrow.Scripting.Wire
{
	[TestFixture]
	public class StaticAndDynamicExpressionTests
	{
		private static readonly StaticParseContext s_StaticContext=CreateStaticContext();
		private static readonly object[] s_StaticArguments=CreateStaticArguments();
		
		private static readonly DynamicParseContext s_DynamicContext=CreateDynamicContext();
		private static readonly IVariableRead s_DynamicScope=CreateDynamicScope();

		[Test]
		public void StaticBitwise()
		{
			ScriptRunner.RunStaticExpressions("Bitwise.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void DynamicBitwise()
		{
			ScriptRunner.RunDynamicExpressions("Bitwise.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticArithmetic()
		{
			ScriptRunner.RunStaticExpressions("Arithmetic.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void DynamicArithmetic()
		{
			ScriptRunner.RunDynamicExpressions("Arithmetic.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticIn()
		{
			ScriptRunner.RunStaticExpressions("In.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void DynamicIn()
		{
			ScriptRunner.RunDynamicExpressions("In.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticRegex()
		{
			ScriptRunner.RunStaticExpressions("Regex.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void DynamicRegex()
		{
			ScriptRunner.RunDynamicExpressions("Regex.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticLike()
		{
			ScriptRunner.RunStaticExpressions("Like.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void DynamicLike()
		{
			ScriptRunner.RunDynamicExpressions("Like.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticNumericTypes()
		{
			ScriptRunner.RunStaticExpressions("NumericTypes.script",s_StaticContext,s_StaticArguments);
		}
		

		[Test]
		public void DynamicNumericTypes()
		{
			ScriptRunner.RunDynamicExpressions("NumericTypes.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticStrings()
		{
			ScriptRunner.RunStaticExpressions("Strings.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicStrings()
		{
			ScriptRunner.RunDynamicExpressions("Strings.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticUnary()
		{
			ScriptRunner.RunStaticExpressions("Unary.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicUnary()
		{
			ScriptRunner.RunDynamicExpressions("Unary.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticLogical()
		{
			ScriptRunner.RunStaticExpressions("Logical.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicLogical()
		{
			ScriptRunner.RunDynamicExpressions("Logical.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticPropertyAndField()
		{
			ScriptRunner.RunStaticExpressions("PropertyAndField.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicPropertyAndField()
		{
			ScriptRunner.RunDynamicExpressions("PropertyAndField.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticRelational()
		{
			ScriptRunner.RunStaticExpressions("Relational.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicRelational()
		{
			ScriptRunner.RunDynamicExpressions("Relational.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticEquality()
		{
			ScriptRunner.RunStaticExpressions("Equality.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicEquality()
		{
			ScriptRunner.RunDynamicExpressions("Equality.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticTernary()
		{
			ScriptRunner.RunStaticExpressions("Ternary.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicTernary()
		{
			ScriptRunner.RunDynamicExpressions("Ternary.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void DynamicConditionalMemberAccess()
		{
			ScriptRunner.RunDynamicExpressions("ConditionalMemberAccess.script",s_DynamicContext,s_DynamicScope);
		}

		[Test]
		public void StaticConditionalMemberAccess()
		{
			ScriptRunner.RunStaticExpressions("ConditionalMemberAccess_static.script",s_StaticContext,s_StaticArguments);
		}

		[Test]
		public void StaticSelect()
		{
			ScriptRunner.RunStaticExpressions("Select.script",s_StaticContext,s_StaticArguments);
		}
		
		[Test]
		public void DynamicSelect()
		{
			ScriptRunner.RunDynamicExpressions("Select.script",s_DynamicContext,s_DynamicScope);
		}


		private static StaticParseContext CreateStaticContext()
		{
			var context=new StaticParseContext();

			context.References.Add(typeof(string).Assembly);
			context.Usings.Add("System");

			context.References.Add(typeof(PropertyAndField).Assembly);
			context.Usings.Add("UnitTests.Arrow.Scripting.Wire.TestClasses");

			context.Parameters.Add(Expression.Parameter(typeof(PropertyAndField),"paf"));

			return context;
		}

		private static object[] CreateStaticArguments()
		{
			object[] arguments={new PropertyAndField()};

			return arguments;
		}

		private static DynamicParseContext CreateDynamicContext()
		{
			var context=new DynamicParseContext();

			context.References.Add(typeof(string).Assembly);
			context.Usings.Add("System");

			context.References.Add(typeof(PropertyAndField).Assembly);
			context.Usings.Add("UnitTests.Arrow.Scripting.Wire.TestClasses");

			return context;
		}

		private static IVariableRead CreateDynamicScope()
		{
			var scope=new LightweightScope();

			scope.Declare("paf",new PropertyAndField());

			return scope;
		}
	}
}
