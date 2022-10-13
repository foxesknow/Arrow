using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution
{
	[TestFixture]
	public class ServiceBusTests
	{
		[Test]
		public void TestRegister()
		{
			ServiceBus bus=new ServiceBus();
		
			bool calledSimple=false;			
			
			EventHandler<EventArgs> simple=(sender,args)=>calledSimple=true;
			bus.Register(simple);
			
			bool calledDerived=false;
			EventHandler<TestEventArgs> derived=(sender,args)=>calledDerived=true;
			bus.Register(derived);
			
			Assert.IsFalse(calledSimple);
			Assert.IsFalse(calledDerived);
			
			bus.Raise(this,new TestEventArgs());
			Assert.IsFalse(calledSimple);
			Assert.IsTrue(calledDerived);
			
			// Reset before next run
			calledDerived=false;
			
			bus.Raise(this,new EventArgs());
			Assert.IsTrue(calledSimple);
			Assert.IsFalse(calledDerived);
		}
		
		[Test]
		public void TestRemove()
		{
			ServiceBus bus=new ServiceBus();
		
			bool calledSimple=false;
			EventHandler<EventArgs> simple=(sender,args)=>calledSimple=true;
			bus.Register(simple);
			
			bool calledDerived=false;
			EventHandler<TestEventArgs> derived=(sender,args)=>calledDerived=true;
			bus.Register(derived);
			
			Assert.IsFalse(calledSimple);
			Assert.IsFalse(calledDerived);
			
			bus.Raise(this,new TestEventArgs());
			Assert.IsFalse(calledSimple);
			Assert.IsTrue(calledDerived);
			
			// Reset before next run
			calledDerived=false;
			calledSimple=false;
			
			bus.Raise(this,new EventArgs());
			Assert.IsTrue(calledSimple);
			Assert.IsFalse(calledDerived);
			
			// Reset before next run
			calledDerived=false;
			calledSimple=false;
			
			bus.Remove(derived);
			bus.Raise(this,new TestEventArgs());
			Assert.IsFalse(calledDerived);
			Assert.IsFalse(calledSimple);
			
			bus.Raise(this,new EventArgs());
			Assert.IsTrue(calledSimple);
			Assert.IsFalse(calledDerived);
		}
		
		[Test]
		public void TestMultiple()
		{
			ServiceBus bus=new ServiceBus();
		
			int count=0;
			EventHandler<EventArgs> add1=(sender,args)=>count+=1;
			EventHandler<EventArgs> add2=(sender,args)=>count+=2;
			EventHandler<EventArgs> add3=(sender,args)=>count+=3;
			
			bus.Register(add1);
			bus.Register(add2);
			bus.Register(add3);
			
			bus.Raise(this,new EventArgs());
			Assert.That(count,Is.EqualTo(6));
			
			// This shouldn't alter the count
			bus.Raise(this,new TestEventArgs());
			Assert.That(count,Is.EqualTo(6));
			
			count=0;
			bus.Remove(add2);
			bus.Raise(this,new EventArgs());
			Assert.That(count,Is.EqualTo(4));
		}
		
		[Test]
		public void TestRaiseVia()
		{
			Action<Action> raiser=a=>a();
		
			ServiceBus bus=new ServiceBus();
		
			int count=0;
			EventHandler<EventArgs> add1=(sender,args)=>count+=1;
			EventHandler<EventArgs> add2=(sender,args)=>count+=2;
			EventHandler<EventArgs> add3=(sender,args)=>count+=3;
			
			bus.Register(add1);
			bus.Register(add2);
			bus.Register(add3);
			
			bus.Raise(this,new EventArgs());
			Assert.That(count,Is.EqualTo(6));
			
			// This shouldn't alter the count
			bus.RaiseVia(this,new TestEventArgs(),raiser);
			Assert.That(count,Is.EqualTo(6));
			
			count=0;
			bus.Remove(add2);
			bus.RaiseVia(this,new EventArgs(),raiser);
			Assert.That(count,Is.EqualTo(4));
		}
		
		class TestEventArgs : EventArgs{}
	}
}
