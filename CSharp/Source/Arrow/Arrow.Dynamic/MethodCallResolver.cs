using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

using Arrow.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Arrow.Dynamic
{
	public class MethodCallResolver : MethodCallResolverBase
	{
		private MethodCallResolver()
		{
		}

		/// <summary>
		/// Calls a method on an instance
		/// </summary>
		/// <param name="instance">The instance to make the call against</param>
		/// <param name="name">The name of the method to call</param>
		/// <param name="arguments">Any arguments to the method</param>
		/// <returns>An expression that makes the call</returns>
		public static MethodCallExpression Call(Expression instance, string name, BindingFlags bindingFlags, params Expression[] arguments)
		{
			if(instance==null) throw new ArgumentNullException("instance");
			if(name==null) throw new ArgumentNullException("name");

			if(TryMakeCall(instance.Type,instance,name,bindingFlags,arguments,out var callExpression)) return callExpression;
			
			throw new DynamicException("Could not resolve instance call to "+name);
		}

		/// <summary>
		/// Calls a method on an instance
		/// </summary>
		/// <param name="instance">The instance to make the call against</param>
		/// <param name="name">The name of the method to call</param>
		/// <param name="arguments">Any arguments to the method</param>
		/// <returns>An expression that makes the call</returns>
		public static MethodCallExpression Call(Expression instance, string name, BindingFlags bindingFlags, IList<Expression> arguments)
		{
			if(instance==null) throw new ArgumentNullException("instance");
			if(name==null) throw new ArgumentNullException("name");
			if(arguments==null) throw new ArgumentNullException("arguments");

			if(TryMakeCall(instance.Type,instance,name,bindingFlags,arguments,out var callExpression)) return callExpression;
			
			throw new DynamicException("Could not resolve instance call to "+name);
		}

		public static bool TryCall(Expression instance, string name, BindingFlags bindingFlags, IList<Expression> arguments, [NotNullWhen(true)] out MethodCallExpression? callExpression)
		{
			if(instance==null) throw new ArgumentNullException("instance");
			if(name==null) throw new ArgumentNullException("name");
			if(arguments==null) throw new ArgumentNullException("arguments");

			return TryMakeCall(instance.Type,instance,name,bindingFlags,arguments,out callExpression);
		}

		/// <summary>
		/// Calls a static method on a type
		/// </summary>
		/// <param name="type">The type to make the static call against</param>
		/// <param name="name">The name of the method to call</param>
		/// <param name="arguments">Any arguments to the method</param>
		/// <returns>An expression that makes the call</returns>
		public static MethodCallExpression Call(Type type, string name, BindingFlags bindingFlags, params Expression[] arguments)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(name==null) throw new ArgumentNullException("name");
			
			if(TryMakeCall(type,null,name,bindingFlags,arguments,out var callExpression)) return callExpression;
			
			throw new DynamicException("Could not resolve static call to "+name);
		}

		/// <summary>
		/// Calls a static method on a type
		/// </summary>
		/// <param name="type">The type to make the static call against</param>
		/// <param name="name">The name of the method to call</param>
		/// <param name="arguments">Any arguments to the method</param>
		/// <returns>An expression that makes the call</returns>
		public static MethodCallExpression Call(Type type, string name, BindingFlags bindingFlags, IList<Expression> arguments)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(name==null) throw new ArgumentNullException("name");
			
			if(TryMakeCall(type,null,name,bindingFlags,arguments,out var callExpression)) return callExpression;
			
			throw new DynamicException("Could not resolve static call to "+name);
		}

		public static bool TryCall(Type type, string name, BindingFlags bindingFlags, IList<Expression> arguments, [NotNullWhen(true)] out MethodCallExpression? callExpression)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(name==null) throw new ArgumentNullException("name");
			
			return TryMakeCall(type,null,name,bindingFlags,arguments,out callExpression);
		}
		

		private static bool TryMakeCall(Type type, Expression? instance, string name, BindingFlags flags, IList<Expression> arguments, [NotNullWhen(true)] out MethodCallExpression? callExpression)
		{
			callExpression=null;

			var methods=GetMethods(type,name,flags);
			if(methods.Count==0) 
			{
				// There's no such method
				return false;
			}

			List<MethodInfo> fixedLengthMethods=new List<MethodInfo>();
			List<MethodInfo> variableLengthMethods=new List<MethodInfo>();

			FilterMethods(arguments.Count,methods,fixedLengthMethods,variableLengthMethods);
			if(fixedLengthMethods.Count==0 && variableLengthMethods.Count==0) 
			{
				string error=string.Format("no matching methods for {0}.{1}({2})",type,name,MakeTypeString(arguments));
				throw new DynamicException(error);
			}

			// First look for an exact match. Anything more than 1 is invalid
			var exactMatch=FindMethods(fixedLengthMethods,arguments,ParameterMatchType.Exact);
			if(exactMatch.Count==1) 
			{
				callExpression=exactMatch[0].Make(type,instance);
				return true;
			}
			else if(exactMatch.Count>1) 
			{
				// It's ambigious
				return false;
			}

			// Now look for best matches
			var bestMatch=FindMethods(fixedLengthMethods,arguments,ParameterMatchType.Cast);
			var fixedMatch=FindBestMatch(bestMatch);
			if(fixedMatch!=null) 
			{
				callExpression=fixedMatch.Make(type,instance);
				return true;
			}
			
			// Now try the variable length methods
			var bestVariableMatch=FindMethods(variableLengthMethods,arguments,ParameterMatchType.Cast);
			var variableMatch=FindBestMatch(bestVariableMatch);
			if(variableMatch!=null) 
			{
				callExpression=variableMatch.Make(type,instance);
				return true;
			}

			// Nothing matched :-(
			return false;
		}

		private static List<CallData> FindMethods(IList<MethodInfo> methods, IList<Expression> arguments, ParameterMatchType matchType)
		{
			List<CallData> callData=new List<CallData>();

			foreach(var method in methods)
			{
				int cost;
				var argList=BuildArgumentList(method,arguments,matchType,out cost);
				if(argList!=null) callData.Add(new CallData(method,argList,cost));
			}

			return callData;
		}

		/// <summary>
		/// Attempts to build an argument list for a method call
		/// </summary>
		/// <param name="method">The method to call</param>
		/// <param name="arguments">The arguments to the method</param>
		/// <param name="matchType">How to match the parameters</param>
		/// <param name="cost">Set to the cost of the call</param>
		/// <returns>A new argument list which can be passed to the method on success, otherwise null</returns>
		public static List<Expression>? BuildArgumentList(MethodInfo method, IList<Expression> arguments, ParameterMatchType matchType, out int cost)
		{
			cost=0;

			List<Expression>? callArguments=new List<Expression>();

			var parameters=method.GetParameters();
			bool hasVariable=parameters.HasVariableParameters();
			bool keepProcessing=true;

			for(int i=0; i<parameters.Length && keepProcessing; i++)
			{
				var parameter=parameters[i];
				int paramCost=0;
				Expression? processedArgument=null;
				
				if(hasVariable && i==parameters.Length-1)
				{
					// We've reached the variable (param in C#) part, so convert the rest 
					// into a single parameter which is an array
					processedArgument=ProcessParamsList(parameter,arguments,i,matchType,out paramCost); 
					keepProcessing=false;
				}
				else
				{
					processedArgument=ProcessArgument(parameter.ParameterType,arguments[i],matchType,out paramCost);
				}

				if(processedArgument==null)
				{
					// We couldn't process the argument which
					// means the parameters are invalid for the method
					return null;
				}
				else
				{
					cost+=paramCost;
					callArguments.Add(processedArgument);
				}
			}

			if(arguments.Count!=0 && callArguments.Count==0)
			{
				callArguments=null;
			}

			return callArguments;
		}

		private static NewArrayExpression? ProcessParamsList(ParameterInfo parameter, IList<Expression> args, int startIndex, ParameterMatchType matchType, out int cost)
		{
			cost=0;

			int paramsLength=args.Count-startIndex;
			Type type=parameter.ParameterType.GetElementType()!;

			List<Expression> variableArgs=new List<Expression>();

			for(int i=startIndex; i<args.Count; i++)
			{
				Expression arg=args[i];
				var adjustedArg=ProcessArgument(type,arg,matchType,out cost);
				if(adjustedArg==null) return null;

				variableArgs.Add(adjustedArg);
			}

			return Expression.NewArrayInit(type,variableArgs);
		}

		private static Expression? ProcessArgument(Type paramType, Expression expression, ParameterMatchType matchType, out int cost)
		{
			cost=0;

			Type exprType=expression.Type;

			// Easy. The types are identical
			if(paramType==exprType) return expression;

			// Handle null.
			// We do this regarless of the match type
			if(IsNull(expression) && paramType.CanBeSetToNull()) return expression.ConvertTo(paramType);

			// At this point we need to cast, so stop if we're not allowed to
			if(matchType==ParameterMatchType.Exact) return null;

			var lhs=Type.GetTypeCode(paramType);
			var rhs=Type.GetTypeCode(exprType);
			
			// Work out the default cost of the call
			cost=Math.Abs(((int)lhs)-((int)rhs));

			// They're type compatible
			if(paramType.IsAssignableFrom(exprType)) 
			{
				cost=1;
				return Expression.Convert(expression,paramType);
			}

			// They're numbers and there's an implicit cast available
			if(IsNumericCastImplicit(paramType,exprType)) 
			{
				return Expression.Convert(expression,paramType);
			}

			// Do we need to box?
			if(paramType==typeof(object) && exprType.IsValueType)
			{
				return Expression.Convert(expression,paramType);
			}

			return null;
		}

		private static bool IsNull(Expression expression)
		{
			ConstantExpression? c=expression as ConstantExpression;
			return c!=null && c.Value==null;
		}


		private static string MakeTypeString(IList<Expression> expressions)
		{
			var types=expressions.Select(e=>e.Type.ToString());
			return string.Join(",",types);
		}
	}
}
