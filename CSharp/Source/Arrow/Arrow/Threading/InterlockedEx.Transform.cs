using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// Useful methods based around the System.Threading.Interlocked class
	/// </summary>
	public partial class InterlockedEx
	{
		/// <summary>
		/// Transforms an integer
		/// </summary>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to transform the value</param>
		/// <returns>The previous value in target</returns>
		public static int Transform(ref int target, Func<int,int> transformer)
		{
			int current;
			int existing=target;
			
			do
			{
				current=existing;
				int newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}
		
		/// <summary>
		/// Transforms an integer
		/// </summary>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to morph the value</param>
		/// <returns>The previous value in target</returns>
		public static long Transform(ref long target, Func<long,long> transformer)
		{
			long current;
			long existing=target;
			
			do
			{
				current=existing;
				long newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}

		/// <summary>
		/// Transforms a float
		/// </summary>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to transform the value</param>
		/// <returns>The previous value in target</returns>
		public static double Transform(ref float target, Func<float,float> transformer)
		{
			float current;
			float existing=target;
			
			do
			{
				current=existing;
				float newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}

		/// <summary>
		/// Transforms a double
		/// </summary>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to transform the value</param>
		/// <returns>The previous value in target</returns>
		public static double Transform(ref double target, Func<double,double> transformer)
		{
			double current;
			double existing=target;
			
			do
			{
				current=existing;
				double newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}

		/// <summary>
		/// Transforms an IntPtr
		/// </summary>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to transform the value</param>
		/// <returns>The previous value in target</returns>
		public static IntPtr Transform(ref IntPtr target, Func<IntPtr,IntPtr> transformer)
		{
			IntPtr current;
			IntPtr existing=target;
			
			do
			{
				current=existing;
				IntPtr newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}
		
		/// <summary>
		/// Transforms an integer
		/// </summary>
		/// <typeparam name="T">The type of the value to set</typeparam>
		/// <param name="target">The address to set</param>
		/// <param name="transformer">A function to morph the value</param>
		/// <returns>The previous value in target</returns>
		public static T Transform<T>(ref T target, Func<T,T> transformer) where T:class
		{
			T current;
			T existing=target;
			
			do
			{
				current=existing;
				T newValue=transformer(current);
				existing=Interlocked.CompareExchange(ref target,newValue,current);
			}while(current!=existing);
			
			return existing;
		}
	}
}
