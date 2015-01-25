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
	public class TaskWorkQueueTests
	{
		[Test]
		public void ReturnsValue()
		{
			using(var queue=new TaskWorkQueue())
			{
				var task=queue.Enqueue(()=>
				{
					return 10;
				});

				Assert.That(task,Is.Not.Null);
				Assert.That(task.Result,Is.EqualTo(10));
			}
		}

		[Test]
		public void ReturnsVoid()
		{
			int result=0;

			using(var queue=new TaskWorkQueue())
			{
				var task=queue.Enqueue(()=>
				{
					result=10;
				});

				Assert.That(task,Is.Not.Null);
				
				// Wait for the task to finish before checking the result
				task.Wait();
				Assert.That(result,Is.EqualTo(10));
			}
		}

		[Test]
		public void ThrowsException()
		{
			using(var queue=new TaskWorkQueue())
			{
				var task=queue.Enqueue(()=>
				{
					throw new InvalidOperationException();
				});

				Assert.That(task,Is.Not.Null);
				
				// Wait for the task to finish before checking the result
				var exception=Assert.Throws<AggregateException>(()=>
				{
					task.Wait();
				});

				Assert.That(exception.InnerException,Is.InstanceOf<InvalidOperationException>());
			}
		}
	}
}
