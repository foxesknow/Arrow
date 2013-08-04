using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class AlertableEventQueueTests
	{
		[Test]
		public void DefaultInitialization()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{
				Assert.That(names.Count,Is.EqualTo(0));
				
				// The wait handle shouldn't be signalled
				bool signalled=names.AvailableHandle.WaitOne(0,false);
				Assert.That(signalled,Is.False);
			}
		}
		
		[Test]
		public void SequenceInitialization()
		{
			List<string> people=new	List<string>(){"Jack","James","Kate"};
		
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>(people))
			{
				Assert.That(names.Count,Is.EqualTo(3));
				
				// The wait handle should be signalled
				bool signalled=names.AvailableHandle.WaitOne(0,false);
				Assert.That(signalled,Is.True);
			}
		}
		
		[Test]
		public void Enqueue()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{			
				names.Enqueue("Hurley");
				Assert.That(names.Count,Is.EqualTo(1));
				
				// The wait handle should be signalled
				bool signalled=names.AvailableHandle.WaitOne(0,false);
				Assert.That(signalled,Is.True);
			}
		}
		
		[Test]
		public void DequeueOne()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{		
				names.Enqueue("Hurley");
				
				string name=names.Dequeue();
				Assert.That(name,Is.EqualTo("Hurley"));
				
				Assert.That(names.Count,Is.EqualTo(0));
				
				// The wait handle shouldn't be signalled
				bool signalled=names.AvailableHandle.WaitOne(0,false);
				Assert.That(signalled,Is.False);
			}
		}
		
		[Test]
		public void DequeueMany()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{			
				names.Enqueue("Hurley");
				names.Enqueue("Locke");
				names.Enqueue("Ben");
				
				Assert.That(names.Dequeue(),Is.EqualTo("Hurley"));			
				Assert.That(names.Count,Is.EqualTo(2));
				
				Assert.That(names.Dequeue(),Is.EqualTo("Locke"));			
				Assert.That(names.Count,Is.EqualTo(1));
				
				Assert.That(names.Dequeue(),Is.EqualTo("Ben"));			
				Assert.That(names.Count,Is.EqualTo(0));
			}
		}
		
		[Test]
		public void TryDequeueOnEmpty()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{			
				string name;
				bool gotData=names.TryDequeue(out name);
				Assert.That(gotData,Is.False);
				Assert.That(name,Is.Null);
			}
		}
		
		[Test]
		public void TryDequeue()
		{
			using(AlertableEventQueue<string> names=new AlertableEventQueue<string>())
			{
				names.Enqueue("Sun");
				
				string name;
				bool gotData=names.TryDequeue(out name);
				Assert.That(gotData,Is.True);
				Assert.That(name,Is.EqualTo("Sun"));
			}
		}
	}
}
