using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Threading.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Collections
{
	[TestFixture]
	public class SwapDictionaryTests
	{
		[Test]
		public void Initialization()
		{
			var dict=new SwapDictionary<int,string>();

			Assert.That(dict.Count,Is.EqualTo(0));
		}

		[Test]
		public void Add()
		{
			var dict=new SwapDictionary<int,string>();
			Assert.That(dict.Count,Is.EqualTo(0));

			OnThreads(()=>dict.Add(1,"Jack"),()=>dict.Add(2,"Kate"));
			Assert.That(dict.Count,Is.EqualTo(2));

			Assert.That(dict.ContainsKey(1),Is.True);
			Assert.That(dict.ContainsKey(2),Is.True);
		}

		[Test]
		public void AddByIndex()
		{
			var dict=new SwapDictionary<int,string>();
			Assert.That(dict.Count,Is.EqualTo(0));

			OnThreads(()=>dict[1]="Jack",()=>dict[2]="Kate");
			Assert.That(dict.Count,Is.EqualTo(2));

			Assert.That(dict.ContainsKey(1),Is.True);
			Assert.That(dict.ContainsKey(2),Is.True);
		}

		[Test]
		public void Clear()
		{
			var dict=new SwapDictionary<int,string>();
			dict.Add(1,"Jack");
			dict.Add(2,"Kate");

			OnThreads(()=>dict.Clear(),()=>dict.Clear());
			Assert.That(dict.Count,Is.EqualTo(0));
		}

		[Test]
		public void ContainsKey()
		{
			var dict=new SwapDictionary<int,string>();
			OnThreads(()=>dict[1]="Jack",()=>dict[2]="Kate");

			Assert.That(dict.ContainsKey(1),Is.True);
			Assert.That(dict.ContainsKey(2),Is.True);
		}

		[Test]
		public void Keys()
		{
			var dict=new SwapDictionary<int,string>();
			OnThreads(()=>dict[1]="Jack",()=>dict[2]="Kate");

			Assert.That(dict.Keys.Count(),Is.EqualTo(2));
			Assert.That(dict.Keys.Contains(2),Is.True);
		}

		[Test]
		public void Values()
		{
			var dict=new SwapDictionary<int,string>();
			OnThreads
			(
				()=>dict[1]="Jack",
				()=>dict[2]="Kate",
				()=>dict[58]="Sawyer"
			);

			Assert.That(dict.Values.Count(),Is.EqualTo(3));
			Assert.That(dict.Values.Contains("Sawyer"),Is.True);
		}

		[Test]
		public void Remove()
		{
			var dict=new SwapDictionary<int,string>();
			OnThreads(()=>dict[1]="Jack",()=>dict[2]="Kate");
		}

		
		[Test]
		public void TryGetValue()
		{
			var dict=new SwapDictionary<int,string>();
			OnThreads
			(
				()=>dict[1]="Jack",
				()=>dict[2]="Kate",
				()=>dict[58]="Sawyer"
			);

			string value=null;
			Assert.That(dict.TryGetValue(2,out value),Is.True);
			Assert.That(value,Is.EqualTo("Kate"));

			Assert.That(dict.TryGetValue(58,out value),Is.True);
			Assert.That(value,Is.EqualTo("Sawyer"));

			Assert.That(dict.TryGetValue(22,out value),Is.False);
			Assert.That(value,Is.Null);
		}

		[Test]
		public void GetValueOrAdd()
		{
			bool called=false;
			var dict=new SwapDictionary<int,string>();

			string result1=dict.GetValueOrAdd(1,key=>
			{
				called=true;
				return "Hurley";
			});

			Assert.That(called,Is.True);
			Assert.That(result1,Is.EqualTo("Hurley"));

			called=false;
			string result2=dict.GetValueOrAdd(1,key=>
			{
				called=true;
				return "Hurley";
			});

			Assert.That(called,Is.False);
			Assert.That(result2,Is.EqualTo("Hurley"));
		}

		private void OnThreads(params Action[] actions)
		{
			Thread[] threads=new Thread[actions.Length];

			for(int i=0; i<actions.Length; i++)
			{
				Action action=actions[i];
				threads[i]=new Thread(()=>action());
			}

			for(int i=0; i<actions.Length; i++)
			{
				threads[i].Start();
			}
			
			for(int i=0; i<actions.Length; i++)
			{
				threads[i].Join();
			}
		}
	}
}
