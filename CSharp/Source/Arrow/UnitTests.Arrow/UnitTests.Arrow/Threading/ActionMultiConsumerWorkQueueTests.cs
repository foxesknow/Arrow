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
	public class ActionMultiConsumerWorkQueueTests
	{
		[Test]
		public void Close()
		{
			using(var queue=new ActionMultiConsumerWorkQueue())
			{
				queue.Close();
			}
		}

		[Test]
		public void Add()
		{
			int counter=0;

			using(var queue=new ActionMultiConsumerWorkQueue())
			{
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
				queue.Enqueue(()=>Interlocked.Increment(ref counter));
			}

			Assert.That(counter,Is.EqualTo(8));
		}

		[Test]
		public void RunOne()
		{
			var ids=Run(1);
			Assert.That(ids.Count,Is.EqualTo(1));

			int thisID=Thread.CurrentThread.ManagedThreadId;
			Assert.That(ids.Contains(thisID),Is.False);
		}

		[Test]
		public void RunTwo()
		{
			var ids=Run(2);
			Assert.That(ids.Count,Is.EqualTo(2));

			int thisID=Thread.CurrentThread.ManagedThreadId;
			Assert.That(ids.Contains(thisID),Is.False);
		}

		[Test]
		public void RunProcessorCount()
		{
			var cores=Environment.ProcessorCount;
			var ids=Run(cores);
			Assert.That(ids.Count,Is.EqualTo(cores));

			int thisID=Thread.CurrentThread.ManagedThreadId;
			Assert.That(ids.Contains(thisID),Is.False);
		}

		[Test]
		public void RunTwiceProcessorCount()
		{
			var cores=Environment.ProcessorCount*2;
			var ids=Run(cores);
			Assert.That(ids.Count,Is.EqualTo(cores));

			int thisID=Thread.CurrentThread.ManagedThreadId;
			Assert.That(ids.Contains(thisID),Is.False);
		}
		
		private HashSet<int> Run(int numberOfConsumers)
		{
			HashSet<int> threadIDs=new HashSet<int>();

			using(var startRunning=new OutstandingEvent())
			using(var queue=new ActionMultiConsumerWorkQueue(numberOfConsumers))
			{
				startRunning.Increase(numberOfConsumers);

				for(int i=0; i<numberOfConsumers; i++)
				{
					queue.Enqueue(()=>
					{
						startRunning.Decrease();
						startRunning.NothingOutstandingHandle.WaitOne();

						lock(threadIDs)
						{
							threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
						}
					});
				}
			}
			 
			return threadIDs;
		}
	}
}
