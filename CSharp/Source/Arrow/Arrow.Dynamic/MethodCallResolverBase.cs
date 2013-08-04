using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Arrow.Reflection;

namespace Arrow.Dynamic
{
	public abstract class MethodCallResolverBase
	{
		internal MethodCallResolverBase()
		{
		}

		/// <summary>
		/// Examines a series of methods and works out which ones can be called
		/// </summary>
		/// <param name="arity">The number of paramters we're looking for</param>
		/// <param name="methods">The methods to examine</param>
		/// <param name="fixedParams">Populated with methods of fixed paramter length</param>
		/// <param name="variableParams">Populates with methods of variable lengh (param)</param>
		protected static void FilterMethods(int arity, IList<MethodInfo> methods, IList<MethodInfo> fixedParams, IList<MethodInfo> variableParams)
		{
			foreach(var method in methods)
			{
				// Ignore generics (for now)
				if(method.ContainsGenericParameters) continue;

				var parameters=method.GetParameters();

				int methodArity=parameters.Length;
				bool hasVariable=parameters.HasVariableParameters();

				if(hasVariable==false && arity!=methodArity) continue;

				if(hasVariable==false && arity==methodArity)
				{
					fixedParams.Add(method);
				}
				else
				{
					// It's a methods with a variable number of parameters
					// We only need to match up to the params, so reduce the arity by 1
					methodArity--;
					if(methodArity<arity)
					{
						variableParams.Add(method);
					}
				}
			}
		}

		/// <summary>
		/// Determines the best call match
		/// </summary>
		/// <typeparam name="T">The type of the call data to examine</typeparam>
		/// <param name="candidates">The possible candidates to call</param>
		/// <returns>The CallData to use, or null if there isn't a best match</returns>
		protected static T FindBestMatch<T>(List<T> candidates) where T:ICallData
		{
			// Easy, there's only one so it must be the one
			if(candidates.Count==1) return candidates[0];

			T callData=default(T);
			
			candidates.Sort((lhs,rhs)=>lhs.Cost.CompareTo(rhs.Cost));
			
			if(candidates.Count>1)
			{
				if(candidates[0].Cost==candidates[1].Cost)
				{
					// Two methods have the same cost, which makes the call ambigious
					callData=default(T);
				}
				else
				{
					// Although there's multiple candidates there's a clear winner
					callData=candidates[0];
				}
			}

			return callData;
		}

				/// <summary>
		/// Work out if an implicit cast (ie a short to a long) can be performed
		/// </summary>
		/// <param name="target"></param>
		/// <param name="numericType"></param>
		/// <returns></returns>
		protected static bool IsNumericCastImplicit(Type target, Type numericType)
		{
			TypeCode targetCode=Type.GetTypeCode(target);
			TypeCode numericCode=Type.GetTypeCode(numericType);

			bool implicitCast=false;

			if(NumbersCLS.IsNumeric(targetCode) && NumbersCLS.IsNumeric(numericCode)&& targetCode>=numericCode)
			{
				implicitCast=true;
			}

			return implicitCast;
		}

		/// <summary>
		/// Gets all the methods that match a name and binding
		/// </summary>
		/// <param name="type">The type to query</param>
		/// <param name="name">The name of the method to look for</param>
		/// <param name="flags">How to select the method</param>
		/// <returns>A list of methods that match the name and flags</returns>
		protected static List<MethodInfo> GetMethods(Type type, string name, BindingFlags flags)
		{
			var methods=GrabMethods(type,name,flags);

			var allMethods=new List<MethodInfo>(methods);

			if(allMethods.Count==0 && type.IsInterface)
			{
				// It the type was actually an interface then check against Object
				var objectMethods=GrabMethods(typeof(object),name,flags);
				allMethods.AddRange(objectMethods);
			}

			return allMethods;
		}

		protected static IEnumerable<MethodInfo> GrabMethods(Type type, string name, BindingFlags flags)
		{
			var methods=from method in type.GetMethods(flags)
						where method.IsClsCompliant() && string.Compare(method.Name,name,true)==0
						select method;

			return methods;
		}

	}
}
