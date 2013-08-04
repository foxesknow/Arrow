using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arrow.Reflection
{
	/// <summary>
	/// Useful extension methods for the MethodInfo, ParameterInfo etc classes
	/// </summary>
	public static class TypeInfoExtensions
	{
		/// <summary>
		/// Determines if the parameters represent a variable number of parameters
		/// </summary>
		/// <param name="parametersInfo">The parameters to check</param>
		/// <returns>true is the parameter list is variable, false otherwise</returns>
		public static bool HasVariableParameters(this IList<ParameterInfo> parametersInfo)
		{
			if(parametersInfo==null) throw new ArgumentNullException("parametersInfo");
			
			if(parametersInfo.Count==0) return false;
			
			ParameterInfo lastParameter=parametersInfo[parametersInfo.Count-1];
			return lastParameter.IsDefined(typeof(ParamArrayAttribute),true);
		}

		/// <summary>
		/// Determines if a parameter represents a param sequence
		/// </summary>
		/// <param name="parameter">The parameter to check</param>
		/// <returns>true if the parameter is a param, otherwise false</returns>
		public static bool IsVariableParameter(this ParameterInfo parameter)
		{
			if(parameter==null) throw new ArgumentNullException("parameter");

			return parameter.IsDefined(typeof(ParamArrayAttribute),true);
		}

		/// <summary>
		/// Determines if a method is CLS compliant
		/// </summary>
		/// <param name="methodBase">The method to check</param>
		/// <returns>true if compliant, otherwise false</returns>
		public static bool IsClsCompliant(this MethodBase methodBase)
		{
			bool compliant=true;
			
			CLSCompliantAttribute[] attr=(CLSCompliantAttribute[])methodBase.GetCustomAttributes(typeof(CLSCompliantAttribute),true);
			if(attr.Length!=0)
			{
				compliant=attr[0].IsCompliant;
			}
			
			return compliant;
		}
	}
}
