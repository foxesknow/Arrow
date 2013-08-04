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
	public class SwapListTests
	{
		[Test]
		public void Initialize()
		{
			var list=new SwapList<int>();
			Assert.That(list.Count,Is.EqualTo(0));
		}

		[Test]
		public void InitializeFromSequence()
		{
			int[] numbers={1,1,2,3,5,8};

			var list=new SwapList<int>(numbers);
			Assert.That(list.Count,Is.EqualTo(6));
		}

		[Test]
		public void Add()
		{
			var list=new SwapList<int>();

			OnThreads
			(
				()=>list.Add(1),
				()=>list.Add(1),
				()=>list.Add(2),
				()=>list.Add(3)
			);

			Assert.That(list.Count,Is.EqualTo(4));
			Assert.That(list,Has.Member(2));
		}

		[Test]
		public void AddRange()
		{
			var list=new SwapList<int>();

			OnThreads
			(
				()=>list.AddRange(new int[]{1,2,3}),
				()=>list.AddRange(new int[]{11,12,13})
			);

			Assert.That(list.Count,Is.EqualTo(6));
			Assert.That(list,Has.Member(2));
			Assert.That(list,Has.Member(13));
		}

		[Test]
		public void Clear()
		{
			var list=new SwapList<int>(){1,2,3,4,5,6};
			Assert.That(list.Count,Is.EqualTo(6));

			list.Clear();
			Assert.That(list.Count,Is.EqualTo(0));
		}

		[Test]
		public void IndexOf()
		{
			var list=new SwapList<int>(){1,2,3,4,5,6};
			
			Assert.That(list.IndexOf(2),Is.EqualTo(1));
			Assert.That(list.IndexOf(99),Is.EqualTo(-1));
		}

		[Test]
		public void Insert()
		{
			var list=new SwapList<int>(){1,2,3,4,5,6};
			
			OnThreads
			(
				()=>list.Insert(0,99),
				()=>list.Insert(0,100)
			);

			Assert.That(list.Count,Is.EqualTo(8));
			Assert.That(list,Has.Member(99));
			Assert.That(list,Has.Member(100));

			// We don't know for sure which item will go
			// into index 0, but whichever one does the other
			// will be in index 1
			Assert.That(list[0]==99 || list[0]==100,Is.True);
			Assert.That(list[1]==99 || list[1]==100,Is.True);
		}

		[Test]
		public void IndexAccess()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list[5],Is.EqualTo(8));

			list[0]=99;
			Assert.That(list[0],Is.EqualTo(99));
		}

		[Test]
		public void Contains()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			Assert.That(list.Contains(8),Is.True);
			Assert.That(list.Contains(88),Is.False);
		}

		[Test]
		public void ToArray()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			var data=list.ToArray();
			Assert.That(data,Is.Not.Null);
			Assert.That(data.Length,Is.EqualTo(6));

			for(int i=0; i<data.Length; i++)
			{
				Assert.That(data[i],Is.EqualTo(list[i]));
			}
		}

		[Test]
		public void Count()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.Count,Is.EqualTo(6));

			list.Clear();
			Assert.That(list.Count,Is.EqualTo(0));
		}

		[Test]
		public void IsReadOnly()
		{
			var list=new SwapList<int>();
			Assert.That(list.IsReadOnly,Is.False);
		}

		[Test]
		public void Remove()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			OnThreads
			(
				()=>list.Remove(2),
				()=>list.Remove(8)
			);

			Assert.That(list.Count,Is.EqualTo(4));

			Assert.That(list.Remove(99),Is.False);
			Assert.That(list.Count,Is.EqualTo(4));

			Assert.That(list.Remove(5),Is.True);
			Assert.That(list.Count,Is.EqualTo(3));
		}

		[Test]
		public void RemoveAt()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			OnThreads
			(
				()=>list.RemoveAt(0),
				()=>list.RemoveAt(0)
			);

			Assert.That(list.Count,Is.EqualTo(4));
			Assert.That(list.Contains(1),Is.False);
		}

		[Test]
		public void RemoveAll()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			OnThreads
			(
				()=>list.RemoveAll(x=>x==1),
				()=>list.RemoveAll(x=>x==3)
			);

			Assert.That(list.Count,Is.EqualTo(3));
			Assert.That(list.Contains(1),Is.False);
			Assert.That(list.Contains(3),Is.False);
			Assert.That(list[0],Is.EqualTo(2));
		}

		[Test]
		public void Exists()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.Exists(x=>x==8),Is.True);
			Assert.That(list.Exists(x=>x==88),Is.False);
		}

		[Test]
		public void BinarySearch()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.BinarySearch(8),Is.EqualTo(5));
			Assert.That(list.BinarySearch(4),Is.LessThan(0));
		}

		[Test]
		public void Find()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.Find(x=>x==2),Is.EqualTo(2));
			Assert.That(list.Find(x=>x==99),Is.EqualTo(0));
		}

		[Test]
		public void FindAll()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			var results=list.FindAll(x=>x>3);
			Assert.That(results,Is.Not.Null);
			Assert.That(results.Count,Is.EqualTo(2));

			var noMatches=list.FindAll(x=>x>99);
			Assert.That(noMatches,Is.Not.Null);
			Assert.That(noMatches.Count,Is.EqualTo(0));
		}

		[Test]
		public void FindIndex()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.FindIndex(x=>x==8),Is.EqualTo(5));
			Assert.That(list.FindIndex(x=>x==0),Is.EqualTo(-1));
		}

		[Test]
		public void ConvertAll()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			var converted=list.ConvertAll(x=>x*2);

			Assert.That(list.Count,Is.EqualTo(converted.Count));

			for(int i=0; i<list.Count; i++)
			{
				Assert.That(list[i]*2,Is.EqualTo(converted[i]));
			}
		}

		[Test]
		public void TrueForAll()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			Assert.That(list.TrueForAll(x=>x>0),Is.True);
			Assert.That(list.TrueForAll(x=>x<0),Is.False);
		}

		[Test]
		public void ForEach()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};

			int total=0;
			list.ForEach(x=>total+=x);
			Assert.That(total,Is.EqualTo(20));
		}

		[Test]
		public void Sort()
		{
			var list=new SwapList<int>(){8,5,2,3,1,1};
			list.Sort();

			Assert.That(list[0],Is.EqualTo(1));
			Assert.That(list[1],Is.EqualTo(1));
			Assert.That(list[2],Is.EqualTo(2));
			Assert.That(list[3],Is.EqualTo(3));
			Assert.That(list[4],Is.EqualTo(5));
			Assert.That(list[5],Is.EqualTo(8));
		}

		[Test]
		public void Reverse()
		{
			var list=new SwapList<int>(){7,11,10};
			list.Reverse();

			Assert.That(list[0],Is.EqualTo(10));
			Assert.That(list[1],Is.EqualTo(11));
			Assert.That(list[2],Is.EqualTo(7));
		}

		[Test]
		public void Enumerator()
		{
			var list=new SwapList<int>(){1,1,2,3,5,8};
			Assert.That(list.Max(),Is.EqualTo(8));
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
