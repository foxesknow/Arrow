using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Arrow.Execution
{
	/// <summary>
	/// Measures how long a block of code takes to run
	/// </summary>
	public static class ExecutionTimer
	{
		private static readonly double s_Frequency;
		
		private static readonly double s_MillisecondFrequency;
		private static readonly double s_MicrosecondFrequency;
		private static readonly double s_NanosecondFrequency;
		
		static ExecutionTimer()
		{
			var frequency = Stopwatch.Frequency;
			s_Frequency=frequency;
			
			// QueryPerformanceFrequency gives the number of counts per second.
			// We need to scale it down for higher resolutions.
			s_MillisecondFrequency = s_Frequency / 1000;
			s_MicrosecondFrequency = s_Frequency / 1000000;
			s_NanosecondFrequency = s_Frequency / 1000000000;
		}
		
		/// <summary>
		/// Measures the time it takes to run a block
		/// </summary>
		/// <param name="mode">How to measure the time taken</param>
		/// <param name="block">The block to run</param>
		/// <returns>The time it took to run block in whatever time units were requested</returns>
		public static double Measure(TimerMode mode, Action block)
		{
			long start = Now;
			block();
			long stop = Now;
			
			switch(mode)
			{
				case TimerMode.Milliseconds:
					return ElapsedMilliseconds(start, stop);
					
				case TimerMode.Microseconds:
					return ElapsedMicroseconds(start, stop);
					
				case TimerMode.Nanoseconds:
					return ElapsedNanoseconds(start, stop);
					
				default:
					throw new ArgumentException("Invalid mode: " + mode.ToString());
			}
		}

		/// <summary>
		/// Measures the time it takes to run a block
		/// </summary>
		/// <param name="mode">How to measure the time taken</param>
		/// <param name="state">State data to pass to the block</param>
		/// <param name="block">The block to run</param>
		/// <returns>The time it took to run block in whatever time units were requested</returns>
		public static double Measure<TState>(TimerMode mode, TState state, Action<TState> block)
		{
			long start = Now;
			block(state);
			long stop = Now;
			
			switch(mode)
			{
				case TimerMode.Milliseconds:
					return ElapsedMilliseconds(start, stop);
					
				case TimerMode.Microseconds:
					return ElapsedMicroseconds(start, stop);
					
				case TimerMode.Nanoseconds:
					return ElapsedNanoseconds(start, stop);
					
				default:
					throw new ArgumentException("Invalid mode: " + mode.ToString());
			}
		}
		
		/// <summary>
		/// Takes a time measurement.
		/// If you sample two time measurements via this property then you can
		/// subtract the two to get an opaque value representing the elapsed time.
		/// </summary>
		/// <returns>An opaque value representing the time now</returns>
		public static long Now
		{
			get
			{
				long start = Stopwatch.GetTimestamp();
				return start;
			}
		}
		
		/// <summary>
		/// Calcualtes the number of seconds between the start and stop times
		/// </summary>
		/// <param name="startTime">The start time (from Now)</param>
		/// <param name="stopTime">The stop time (from Now)</param>
		/// <returns>The number of seconds between the times</returns>
		public static double ElapsedSeconds(long startTime, long stopTime)
		{
			double elapsed = (stopTime - startTime) / s_Frequency;
			return elapsed;
		}
		
		/// <summary>
		/// Returns the number of elapsed seconds
		/// </summary>
		/// <param name="totalElapsedTime">The amount of elapsed time</param>
		/// <returns>The number of seconds represented by the elapsed time</returns>
		public static double ElapsedSeconds(long totalElapsedTime)
		{
			return totalElapsedTime / s_Frequency;
		}
		
		/// <summary>
		/// Calcualtes the number of milliseconds between the start and stop times
		/// </summary>
		/// <param name="startTime">The start time (from Now)</param>
		/// <param name="stopTime">The stop time (from Now)</param>
		/// <returns>The number of milliseconds between the times</returns>
		public static double ElapsedMilliseconds(long startTime, long stopTime)
		{
			double elapsed = (stopTime - startTime) / s_MillisecondFrequency;
			return elapsed;
		}
		
		/// <summary>
		/// Returns the number of elapsed milliseconds
		/// </summary>
		/// <param name="totalElapsedTime">The amount of elapsed time</param>
		/// <returns>The number of milliseconds represented by the elapsed time</returns>
		public static double ElapsedMilliseconds(long totalElapsedTime)
		{
			return totalElapsedTime / s_MillisecondFrequency;
		}
		
		/// <summary>
		/// Calcualtes the number of microseconds between the start and stop times
		/// </summary>
		/// <param name="startTime">The start time (from Now)</param>
		/// <param name="stopTime">The stop time (from Now)</param>
		/// <returns>The number of microseconds between the times</returns>
		public static double ElapsedMicroseconds(long startTime, long stopTime)
		{
			double elapsed = (stopTime - startTime) / s_MicrosecondFrequency;
			return elapsed;
		}
		
		/// <summary>
		/// Returns the number of elapsed microseconds
		/// </summary>
		/// <param name="totalElapsedTime">The amount of elapsed time</param>
		/// <returns>The number of microseconds represented by the elapsed time</returns>
		public static double ElapsedMicroseconds(long totalElapsedTime)
		{
			return totalElapsedTime / s_MicrosecondFrequency;
		}
		
		/// <summary>
		/// Calcualtes the number of nanoseconds between the start and stop times
		/// </summary>
		/// <param name="startTime">The start time (from Now)</param>
		/// <param name="stopTime">The stop time (from Now)</param>
		/// <returns>The number of nanoseconds between the times</returns>
		public static double ElapsedNanoseconds(long startTime, long stopTime)
		{
			double elapsed = (stopTime - startTime) / s_NanosecondFrequency;
			return elapsed;
		}
		
		/// <summary>
		/// Returns the number of elapsed nanoseconds
		/// </summary>
		/// <param name="totalElapsedTime">The amount of elapsed time</param>
		/// <returns>The number of nanoseconds represented by the elapsed time</returns>
		public static double ElapsedNanoseconds(long totalElapsedTime)
		{
			return totalElapsedTime / s_NanosecondFrequency;
		}
	}
}
