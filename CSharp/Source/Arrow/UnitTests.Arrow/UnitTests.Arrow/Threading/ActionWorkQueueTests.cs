using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Threading;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading
{
	[TestFixture]
	public class ActionWorkQueueTests
	{
		[Test]
		public void NoActions()
		{
			using(var queue=new ActionWorkQueue())
			{
			}
			
			// We've never created a thread (above) so we should get here without any blocking
		}
	
		[Test]
		public void OneAction()
		{
			long flag=0;
			
			Action action=()=>Interlocked.Increment(ref flag);
		
			using(var queue=new ActionWorkQueue())
			{
				queue.Enqueue(action);
			}
			
			Assert.That(flag,Is.EqualTo(1));
		}
		
		[Test]
		public void TwoActions()
		{
			long flag=0;
			
			Action action1=()=>Interlocked.Increment(ref flag);
			Action action2=()=>Interlocked.Add(ref flag,2);
		
			using(var queue=new ActionWorkQueue())
			{
				queue.Enqueue(action1);
				queue.Enqueue(action2);
			}
			
			Assert.That(flag,Is.EqualTo(3));
		}
		
		[Test]
		public void EnqueueOrExecute_OnCallerThread()
		{
			int currentThreadID=Thread.CurrentThread.ManagedThreadId;			
			int actionThreadID=-1;
			
			using(var queue=new ActionWorkQueue())
			{
				queue.EnqueueOrExecute(()=>actionThreadID=Thread.CurrentThread.ManagedThreadId);
			}
			
			Assert.That(currentThreadID,Is.EqualTo(actionThreadID));
		}		
	}
}
