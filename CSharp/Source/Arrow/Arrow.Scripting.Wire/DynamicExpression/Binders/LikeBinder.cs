using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Arrow.Dynamic;


namespace Arrow.Scripting.Wire.DynamicExpression.Binders
{
	class LikeBinder : BinderBase
	{
		private static readonly MethodInfo ApplyLike=typeof(LikeEvaluator).GetMethod("ApplyLike")!;
		private static readonly MethodInfo ApplyLikeNoCase=typeof(LikeEvaluator).GetMethod("ApplyLikeNoCase")!;

		private readonly CaseMode m_CaseMode;

		public LikeBinder(CaseMode caseMode)
		{
			m_CaseMode=caseMode;
		}

		public override Type ReturnType
		{
			get{return typeof(bool);}
		}

		public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
		{
			Expression? x=null;

			var lhs=target.GetLimitedExpression();
			var pattern=args[0].GetLimitedExpression();

			if(lhs.IsOfType<string>() && pattern.IsOfType<string>())
			{
				x=LikeEvaluator.Like(m_CaseMode,lhs,pattern);
			}
			else
			{
				x=ExpressionEx.ThrowAndReturn<DynamicException,bool>("can only apply like to strings",false);
			}

			var restrictions=BindingRestrictions.Empty.AndLimitType(target);
			return new DynamicMetaObject(x,restrictions);
		}

		private string PatternEncoder(string pattern)
		{
			pattern=Regex.Escape(pattern);
			pattern=pattern.Replace("%",@".*");
			pattern=string.Format("^{0}$",pattern);
			
			return pattern;
		}
	}
}
