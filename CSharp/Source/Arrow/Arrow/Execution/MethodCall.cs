using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Arrow.Execution
{
	/// <summary>
	/// Provides helper methods to call methods
	/// </summary>
	public static class MethodCall
	{
		/// <summary>
		/// Executes a method and ignores any exceptions thrown by the method
		/// </summary>
		/// <param name="method">The method to execute</param>
		/// <exception cref="System.ArgumentNullException">method is null</exception>
		public static void AllowFail(Action method)
		{
			if(method==null) throw new ArgumentNullException("method");
			
			try
			{
				method();
			}
			catch
			{
				// Does nothing
			}
		}

		/// <summary>
		/// Executes a method and ignores any exceptions thrown by the method.
		/// By taking a data parameter this method avoids generating a wrapper class for 
		/// any data used by method, as long as method only used the passed in data
		/// </summary>
		/// <typeparam name="T">The type of data to pass to the method that may fail</typeparam>
		/// <param name="data">The data to pass to the method</param>
		/// <param name="method">The method to call</param>
		public static void AllowFail<T>(T data, Action<T> method)
		{
			if(method==null) throw new ArgumentNullException("method");
			
			try
			{
				method(data);
			}
			catch
			{
				// Does nothing
			}
		}
		
		/// <summary>
		/// Executes a method, calling an optional handler if an exception is thrown
		/// </summary>
		/// <param name="exceptionHandler">An optional handler to call if method throws an exception</param>
		/// <param name="method">The method to execute</param>
		/// <exception cref="System.ArgumentNullException">method is null</exception>
		public static void AllowFail(Action<Exception> exceptionHandler, Action method)
		{
			if(method==null) throw new ArgumentNullException("method");
			
			try
			{
				method();
			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				if(exceptionHandler!=null)
				{
					exceptionHandler(e);
				}
			}
		}
		
		/// <summary>
		/// Attempts to make a method call a number of times before passing the exception onto the caller
		/// </summary>
		/// <param name="numberOfRetries">How many times to retry the operation</param>
		/// <param name="millisecondsBetweenRetries">How long to Thread.Sleep after a failure</param>
		/// <param name="method">The method to call</param>
		/// <exception cref="System.ArgumentException">numberOfRetries is less than 1</exception>
		/// <exception cref="System.ArgumentException">millisecondsBetweenRetries is less than 0</exception>
		/// <exception cref="System.ArgumentNullException">method is null</exception>
		public static void Retry(int numberOfRetries, int millisecondsBetweenRetries, Action method)
		{
			if(numberOfRetries<1) throw new ArgumentException("numberOfRetries must be at least 1");
			if(millisecondsBetweenRetries<0) throw new ArgumentException("millisecondsBetweenRetries");
			if(method==null) throw new ArgumentNullException("method");
			
			while(numberOfRetries--!=0)
			{
				try
				{
					method();
					break;
				}
				catch
				{
					if(numberOfRetries==0)
					{
						// We've had all our chances
						throw;
					}
					
					// Back off before retrying
					Thread.Sleep(millisecondsBetweenRetries);
				}
			}
		}
		
		/// <summary>
		/// Invokes a delegate by checking the ISynchronizeInvoke interface on the target object
		/// </summary>
		/// <param name="delegateToCall">The delegate to call</param>
		/// <param name="parameters">Any parameters to pass to the delegate</param>
		/// <returns>Any value returned by the delegate</returns>
		/// <exception cref="System.ArgumentNullException">delegateToCall is null</exception>
		public static object? Invoke(Delegate delegateToCall, params object[] parameters)
		{
			if(delegateToCall==null) throw new ArgumentNullException("delegateToCall");
			
			var synchronizeInvoke=delegateToCall.Target as ISynchronizeInvoke;
			
			object? result=null;
			
			if(synchronizeInvoke!=null && synchronizeInvoke.InvokeRequired)
			{
				synchronizeInvoke.Invoke(delegateToCall,parameters);
			}
			else
			{
				// Just call straight into the delegate
				result=delegateToCall.DynamicInvoke(parameters);
			}
			
			return result;
		}
		
		/// <summary>
		/// Invokes an event handler by checking the ISynchronizeInvoke interface on the target object.
		/// This method is useful with GUIs for moving an event onto the GUI thread
		/// </summary>
		/// <typeparam name="TEventArgs">An EventArgs derived type</typeparam>
		/// <param name="handler">The event handler to invoke</param>
		/// <param name="sender">The sender of the event</param>
		/// <param name="args">The arguments to pass to the event handler</param>
		/// <exception cref="System.ArgumentNullException">handler is null</exception>
		public static void Invoke<TEventArgs>(EventHandler<TEventArgs> handler, object sender, TEventArgs args) where TEventArgs:EventArgs
		{
			Invoke(handler,new object[]{sender,args});
		}
		
		/// <summary>
		/// Asynchronously invokes a delegate via its targets ISynchronizeInvoke interface
		/// </summary>
		/// <param name="delegateToCall">The delegate to call</param>
		/// <param name="parameters">Any parameters to pass to the delegate</param>
		/// <returns>An sync result instance that can be used to wait for completion of the call</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the target does not support ISynchronizeInvoke</exception>
		/// <exception cref="System.ArgumentNullException">delegateToCall is null</exception>
		public static IAsyncResult BeginInvoke(Delegate delegateToCall, params object[] parameters)
		{
			if(delegateToCall==null) throw new ArgumentNullException("delegateToCall");
			
			var synchronizeInvoke=delegateToCall.Target as ISynchronizeInvoke;
			
			if(synchronizeInvoke==null)
			{
				throw new InvalidOperationException("target object does not implement ISynchronizeInvoke");
			}
			
			return synchronizeInvoke.BeginInvoke(delegateToCall,parameters);
		}
		
		/// <summary>
		/// Asynchronously invokes an event handler by checking the ISynchronizeInvoke interface on the target object.
		/// This method is useful with GUIs for moving an event onto the GUI thread
		/// </summary>
		/// <typeparam name="TEventArgs">An EventArgs derived type</typeparam>
		/// <param name="handler">The event handler to invoke</param>
		/// <param name="sender">The sender of the event</param>
		/// <param name="args">The arguments to pass to the event handler</param>
		/// <returns>An sync result instance that can be used to wait for completion of the call</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the target does not support ISynchronizeInvoke</exception>
		public static IAsyncResult BeginInvoke<TEventArgs>(EventHandler<TEventArgs> handler, object sender, TEventArgs args) where TEventArgs:EventArgs
		{
			return BeginInvoke(handler,new object[]{sender,args});
		}
	}
}
