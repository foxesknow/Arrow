using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Dynamic;

namespace Arrow.Dynamic
{
	/// <summary>
	/// Useful extensions for BindingRestrictions
	/// </summary>
	public static class BindingRestrictionsExtensions
	{
		/// <summary>
		/// Adds a restriction based on type.
		/// When used with a DynamicMetaObject the type is usually the LimitType
		/// </summary>
		/// <param name="restrictions">The existing restrictions to update</param>
		/// <param name="expression">The expression that the restriction applies to</param>
		/// <param name="type">The type the expression must be</param>
		/// <returns>A new restriction</returns>
		public static BindingRestrictions AndType(this BindingRestrictions restrictions, Expression expression, Type type)
		{
			return restrictions.Merge(BindingRestrictions.GetTypeRestriction(expression,type));
		}

		/// <summary>
		/// Adds a restriction based on the limit type of the meta object.
		/// If the object represents null then an instance restriction will be generated
		/// </summary>
		/// <param name="restrictions">The existing restrictions to update</param>
		/// <param name="meta">The meta object to generate the restriction for</param>
		/// <returns>A new restriction</returns>
		public static BindingRestrictions AndLimitType(this BindingRestrictions restrictions, DynamicMetaObject meta)
		{
			if(meta.Value==null) return restrictions.Merge(BindingRestrictions.GetInstanceRestriction(meta.Expression,null));
			
			return restrictions.Merge(BindingRestrictions.GetTypeRestriction(meta.Expression,meta.LimitType));
		}

		/// <summary>
		/// Generates a set of restrictions for a method call.
		/// The 'AndLimitType' extension is used to generate the argument restrictions
		/// </summary>
		/// <param name="restrictions">The existing restrictions to update</param>
		/// <param name="target">The object the call will be made against. May be null</param>
		/// <param name="args">Meta objects that represent arguments to the call</param>
		/// <returns>A new restriction</returns>
		public static BindingRestrictions ForCall(this BindingRestrictions restrictions, DynamicMetaObject? target, DynamicMetaObject[] args)
		{
			if(target!=null) restrictions=restrictions.AndType(target.Expression,target.LimitType);

			foreach(var arg in args)
			{
				restrictions=restrictions.AndLimitType(arg);
			}

			return restrictions;
		}

		/// <summary>
		/// Generates a set of restrictions for all meta objects
		/// The 'AndLimitType' extension is used to generate the argument restrictions
		/// </summary>
		/// <param name="restrictions">The existing restrictions to update</param>
		/// <param name="target">The target</param>
		/// <param name="args">Meta objects that represent arguments to the call</param>
		/// <returns>A new restriction</returns>
		public static BindingRestrictions ForAll(this BindingRestrictions restrictions, DynamicMetaObject target, DynamicMetaObject[] args)
		{
			restrictions=restrictions.AndLimitType(target);

			foreach(var arg in args)
			{
				restrictions=restrictions.AndLimitType(arg);
			}

			return restrictions;
		}
	}
}
