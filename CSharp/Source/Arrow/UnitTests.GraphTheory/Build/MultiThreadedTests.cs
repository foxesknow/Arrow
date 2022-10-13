using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory.Build;

using NUnit.Framework;


namespace UnitTests.Arrow.GraphTheory.Build
{
	[TestFixture]
	public class MultiThreadedTests
	{
		[Test]
		public void LinearTask()
		{
			// None of these tasks can build in parallel
			BuildTask task1=new BuildTask("hello.cpp","stdio.h");
			BuildTask task2=new BuildTask("hello.obj","hello.cpp");
			
			List<IBuildDescription<string>> tasks=new List<IBuildDescription<string>>();
			tasks.Add(task1);
			tasks.Add(task2);
			
			BuildGenerator<string> generator=new BuildGenerator<string>();
			List<ParallelBuildItems<string>> targetOrder=generator.MultiThreadedBuild(tasks);
			Assert.IsNotNull(targetOrder);
			
			Assert.That(targetOrder,Has.Count.EqualTo(3));
			Assert.That(targetOrder[0].Targets.Count,Is.EqualTo(1));
			Assert.That(targetOrder[0].Targets,Has.Member("stdio.h"));
			
			Assert.That(targetOrder[1].Targets.Count,Is.EqualTo(1));
			Assert.That(targetOrder[1].Targets,Has.Member("hello.cpp"));
			
			Assert.That(targetOrder[2].Targets.Count,Is.EqualTo(1));
			Assert.That(targetOrder[2].Targets,Has.Member("hello.obj"));
		}
		
		[Test]
		public void PairTask()
		{
			/*
			 * The parallel runs here are:
			 *  stdio.h
			 *  hello.cpp world.cpp
			 *  hello.obj world.obj
			 *  app.exe
			 */
			BuildTask task1=new BuildTask("hello.cpp","stdio.h");
			BuildTask task2=new BuildTask("hello.obj","hello.cpp");
			BuildTask task3=new BuildTask("world.cpp","stdio.h");
			BuildTask task4=new BuildTask("world.obj","world.cpp");
			BuildTask task5=new BuildTask("app.exe","hello.obj","world.obj");
			
			List<IBuildDescription<string>> tasks=new List<IBuildDescription<string>>();
			tasks.Add(task1);
			tasks.Add(task2);
			tasks.Add(task3);
			tasks.Add(task4);
			tasks.Add(task5);
			
			BuildGenerator<string> generator=new BuildGenerator<string>();
			List<ParallelBuildItems<string>> targetOrder=generator.MultiThreadedBuild(tasks);
			Assert.IsNotNull(targetOrder);
			
			Assert.That(targetOrder,Has.Count.EqualTo(4));
			
			Assert.That(targetOrder[0].Targets.Count,Is.EqualTo(1));
			Assert.That(targetOrder[0].Targets,Has.Member("stdio.h"));
			
			Assert.That(targetOrder[1].Targets.Count,Is.EqualTo(2));
			Assert.That(targetOrder[1].Targets,Has.Member("hello.cpp"));
			Assert.That(targetOrder[1].Targets,Has.Member("world.cpp"));
			
			Assert.That(targetOrder[2].Targets.Count,Is.EqualTo(2));
			Assert.That(targetOrder[2].Targets,Has.Member("hello.obj"));
			Assert.That(targetOrder[2].Targets,Has.Member("world.obj"));
			
			Assert.That(targetOrder[3].Targets.Count,Is.EqualTo(1));
			Assert.That(targetOrder[3].Targets,Has.Member("app.exe"));
			
		}
	}
}
