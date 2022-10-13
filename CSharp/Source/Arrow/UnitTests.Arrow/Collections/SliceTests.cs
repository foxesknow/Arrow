using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class SliceTests
	{
		[Test]
		public void Construction_FromZero()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,0,2);

			Assert.That(slice.Count,Is.EqualTo(2));
			Assert.That(slice[0],Is.EqualTo("Jack"));
			Assert.That(slice[1],Is.EqualTo("Sawyer"));
		}

		[Test]
		public void Construction_FromOne()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,1,2);

			Assert.That(slice.Count,Is.EqualTo(2));
			Assert.That(slice[0],Is.EqualTo("Sawyer"));
			Assert.That(slice[1],Is.EqualTo("Kate"));
		}

		[Test]
		public void Construction_FromTwo()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,2,4);

			Assert.That(slice.Count,Is.EqualTo(4));
			Assert.That(slice[0],Is.EqualTo("Kate"));
			Assert.That(slice[1],Is.EqualTo("Ben"));
			Assert.That(slice[2],Is.EqualTo("Hurley"));
			Assert.That(slice[3],Is.EqualTo("Locke"));
		}

		[Test]
		public void Construction_Everything()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,0,6);

			Assert.That(slice.Count,Is.EqualTo(6));
			Assert.That(slice[0],Is.EqualTo("Jack"));
			Assert.That(slice[1],Is.EqualTo("Sawyer"));
			Assert.That(slice[2],Is.EqualTo("Kate"));
			Assert.That(slice[3],Is.EqualTo("Ben"));
			Assert.That(slice[4],Is.EqualTo("Hurley"));
			Assert.That(slice[5],Is.EqualTo("Locke"));
		}

		[Test]
		public void Construction_BadData()
		{
			var names=CreateNames();

			Assert.Throws<ArgumentException>(()=>
			{
				var slice=new Slice<string>(names,0,7);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				var slice=new Slice<string>(names,-1,2);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				var slice=new Slice<string>(names,-3,2);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				var slice=new Slice<string>(names,1,-2);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				var slice=new Slice<string>(names,1,names.Length);
			});
		}

		[TestCase("Jack",false)]
		[TestCase("Ben",true)]
		[TestCase("Locke",false)]
		public void Contains(string name, bool expected)
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,2,3);

			Assert.That(slice.Contains(name),Is.EqualTo(expected));
		}

		[TestCase("Ben",1)]
		[TestCase("Locke",3)]
		public void IndexOf(string name, int index)
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,2,4);

			Assert.That(slice.IndexOf(name),Is.EqualTo(index));
		}

		[Test]
		public void Index_Set()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,2,4);

			slice[0]="foo";
			Assert.That(slice[0],Is.EqualTo("foo"));
			Assert.That(names[2],Is.EqualTo("foo"));
		}

		[Test]
		public void Index_Set_BadIndex()
		{
			var names=CreateNames();
			var slice=new Slice<string>(names,2,2);

			Assert.Throws<IndexOutOfRangeException>(()=>slice[-1]="foo");
			Assert.Throws<IndexOutOfRangeException>(()=>slice[2]="foo");
		}

		[Test]
		public void AsSlice()
		{
			var names=CreateNames();
			var slice=names.AsSlice(2,3);

			Assert.That(slice,Is.Not.Null);
		}

		[Test]
		public void EnumeratorTests()
		{
			var names=CreateNames();
			var slice=names.AsSlice(2,3);

			Assert.That(slice.FirstOrDefault(x=>x=="Kate"),Is.EqualTo("Kate"));
			Assert.That(slice.FirstOrDefault(x=>x=="Hurley"),Is.EqualTo("Hurley"));
			Assert.That(slice.FirstOrDefault(x=>x=="Jack"),Is.Null);
			Assert.That(slice.FirstOrDefault(x=>x=="Sawyer"),Is.Null);
			Assert.That(slice.FirstOrDefault(x=>x=="Locke"),Is.Null);
		}


		private string[] CreateNames()
		{
			return new string[]{"Jack","Sawyer","Kate","Ben","Hurley","Locke"};
		}
	}
}
