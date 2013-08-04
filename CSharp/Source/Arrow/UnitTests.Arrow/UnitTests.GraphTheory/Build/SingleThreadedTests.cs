using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory.Build;

using NUnit.Framework;


namespace UnitTests.Echo.GraphTheory.Build
{
	[TestFixture]
	public class SingleThreadedTests
	{
		[Test]
		public void LinearTask()
		{
			BuildTask task1=new BuildTask("hello.cpp","stdio.h");
			BuildTask task2=new BuildTask("hello.obj","hello.cpp");
			
			List<IBuildDescription<string>> tasks=new List<IBuildDescription<string>>();
			tasks.Add(task1);
			tasks.Add(task2);
			
			BuildGenerator<string> generator=new BuildGenerator<string>();
			List<string> targetOrder=generator.SingleThreadedBuild(tasks);
			Assert.That(targetOrder,Has.Count.EqualTo(3));
			
			// hello.obj should be the last in the list
			Assert.That(targetOrder[targetOrder.Count-1],Is.EqualTo("hello.obj"));
		}
		
		[Test]
		public void PairTasks()
		{
			
			BuildTask task1=new BuildTask("hello.cpp","stdio.h");
			BuildTask task2=new BuildTask("hello.obj","hello.cpp");
			BuildTask task3=new BuildTask("world.cpp","stdio.h");
			BuildTask task4=new BuildTask("world.obj","world.cpp");
			BuildTask task5=new BuildTask("app.exe","world.obj","hello.obj");
			
			List<IBuildDescription<string>> tasks=new List<IBuildDescription<string>>();
			tasks.Add(task1);
			tasks.Add(task2);
			tasks.Add(task3);
			tasks.Add(task4);
			tasks.Add(task5);
			
			BuildGenerator<string> generator=new BuildGenerator<string>();
			List<string> targetOrder=generator.SingleThreadedBuild(tasks);
			Assert.IsNotNull(targetOrder);
			
			// There should be 6 unique files
			Assert.That(targetOrder,Has.Count.EqualTo(6));
		}
		
		[Test]
		public void Classic()
		{
			// The example date here is taken from the book "Introduction to Algorithms"
			List<IBuildDescription<string>> tasks=new List<IBuildDescription<string>>();
			tasks.Add(new BuildTask("shoes","socks","undershorts","pants"));
			tasks.Add(new BuildTask("watch"));
			tasks.Add(new BuildTask("pants","undershorts"));
			tasks.Add(new BuildTask("belt","pants","shirt"));
			tasks.Add(new BuildTask("tie","shirt"));
			tasks.Add(new BuildTask("jacket","tie","belt"));
			
			BuildGenerator<string> generator=new BuildGenerator<string>();
			List<string> targetOrder=generator.SingleThreadedBuild(tasks);
			Assert.IsNotNull(targetOrder);
			
			// There should be 9 items of clothing
			Assert.That(targetOrder,Has.Count.EqualTo(9));
			
			// The jacket goes on after the belt
			Assert.That(targetOrder.IndexOf("jacket"),Is.GreaterThan(targetOrder.IndexOf("belt")));
			
			// The shoes goes on after the pants
			Assert.That(targetOrder.IndexOf("shoes"),Is.GreaterThan(targetOrder.IndexOf("pants")));
		}
	}
}
