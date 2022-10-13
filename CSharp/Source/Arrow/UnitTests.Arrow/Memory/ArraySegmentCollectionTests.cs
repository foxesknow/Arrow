using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Memory;
using NUnit.Framework;

namespace UnitTests.Arrow.Memory
{
	[TestFixture]
	public class ArraySegmentCollectionTests
	{
		[Test]
		public void Construction_TestChecks()
		{
			Assert.Throws<ArgumentOutOfRangeException>(()=>
			{
				var collection=new ArraySegmentCollection<int>(-1);
			});

			Assert.Throws<ArgumentOutOfRangeException>(()=>
			{
				var collection=new ArraySegmentCollection<int>(-7);
			});

			Assert.Throws<ArgumentNullException>(()=>
			{
				var collection=new ArraySegmentCollection<int>(null);
			});
		}

		[Test]
		public void Construction()
		{
			ArraySegment<int>[] segments=
			{
				new ArraySegment<int>(new int[]{1,1,2,3,5}),
				new ArraySegment<int>(new int[]{8,13,21})
			};

			var collection=new ArraySegmentCollection<int>(segments);
			Assert.That(collection.Count,Is.EqualTo(2));
		}

		[Test]
		public void GetOverallLength()
		{
			var collection=new ArraySegmentCollection<int>();
			Assert.That(collection.GetOverallLength(),Is.EqualTo(0));

			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			Assert.That(collection.GetOverallLength(),Is.EqualTo(5));
			Assert.That(collection.Count,Is.EqualTo(1));

			collection.AddBack(new ArraySegment<int>(new int[]{8,13,21}));
			Assert.That(collection.GetOverallLength(),Is.EqualTo(8));
			Assert.That(collection.Count,Is.EqualTo(2));

			collection.Clear();
			Assert.That(collection.GetOverallLength(),Is.EqualTo(0));
		}

		[Test]
		public void Count()
		{
			var collection=new ArraySegmentCollection<int>();
			Assert.That(collection.Count,Is.EqualTo(0));

			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			Assert.That(collection.Count,Is.EqualTo(1));

			collection.Clear();
			Assert.That(collection.Count,Is.EqualTo(0));
		}

		[Test]
		public void AddBack()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			collection.AddBack(new ArraySegment<int>(new int[]{8,13,21}));

			Assert.That(collection[0].Count,Is.EqualTo(5));
			Assert.That(collection[1].Count,Is.EqualTo(3));
		}

		[Test]
		public void AddFront()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddFront(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			collection.AddFront(new ArraySegment<int>(new int[]{8,13,21}));

			Assert.That(collection[0].Count,Is.EqualTo(3));
			Assert.That(collection[1].Count,Is.EqualTo(5));
		}

		[Test]
		public void Items()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			Assert.That(collection.Items().Sum(),Is.EqualTo(12));

			collection.AddBack(new ArraySegment<int>(new int[]{8,13,21}));
			Assert.That(collection.Items().Sum(),Is.EqualTo(54));
		}

		[Test]
		public void Enumerable()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddFront(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			Assert.That(collection.Count(),Is.EqualTo(1));
			
			collection.AddFront(new ArraySegment<int>(new int[]{8,13,21}));
			Assert.That(collection.Count(),Is.EqualTo(2));

		}

		[Test]
		public void ToArray()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			collection.AddBack(new ArraySegment<int>(new int[]{8,13,21}));

			var array=collection.ToArray();
			Compare(array,0,1,1,2,3,5,8,13,21);
		}

		[Test]
		public void FromArray()
		{
			var collection=ArraySegmentCollection.FromArray(new int[]{1,1,2,3});
			Assert.That(collection,Is.Not.Null);
			Assert.That(collection.Count,Is.EqualTo(1));
			Assert.That(collection.GetOverallLength(),Is.EqualTo(4));
			Compare(collection.ToArray(),0,1,1,2,3);
		}

		[Test]
		public void FromSegment()
		{
			var segment=new ArraySegment<int>(new int[]{1,1,2,3});
			var collection=ArraySegmentCollection.FromSegment(segment);
			Assert.That(collection,Is.Not.Null);
			Assert.That(collection.Count,Is.EqualTo(1));
			Assert.That(collection.GetOverallLength(),Is.EqualTo(4));
			Compare(collection.ToArray(),0,1,1,2,3);
		}

		[Test]
		public void FromMemoryStream()
		{
			using(var stream=new MemoryStream())
			{
				stream.WriteByte(1);
				stream.WriteByte(1);
				stream.WriteByte(2);
				stream.WriteByte(3);

				var collection=ArraySegmentCollection.FromMemoryStream(stream);
				Assert.That(collection,Is.Not.Null);
				Assert.That(collection.Count,Is.EqualTo(1));
				Assert.That(collection.GetOverallLength(),Is.EqualTo(4));
				Compare<byte>(collection.ToArray(),0,1,1,2,3);
			}
		}

		[Test]
		public void CopyIntoSegment()
		{
			var collection=new ArraySegmentCollection<int>();
			collection.AddBack(new ArraySegment<int>(new int[]{1,1,2,3,5}));
			collection.AddBack(new ArraySegment<int>(new int[]{8,13,21}));

			var toSmallDestination=new ArraySegment<int>(new int[3]);
			Assert.Throws<ArgumentException>(()=>
			{
				collection.CopyIntoSegment(toSmallDestination);
			});

			var exactSizeDestination=new ArraySegment<int>(new int[8]);
			collection.CopyIntoSegment(exactSizeDestination);
			Compare(exactSizeDestination.Array,0,1,1,2,3,5,8,13,21);

			var extraSpaceDestination=new ArraySegment<int>(new int[]{7,7,7,7,7,7,7,7,7,7});
			collection.CopyIntoSegment(extraSpaceDestination);
			Compare(extraSpaceDestination.Array,0,1,1,2,3,5,8,13,21);
			Compare(extraSpaceDestination.Array,8,7,7);

			// Copy the collection into a segment that points into the array
			var oversizedArray=new int[]{7,7,7,7,7,7,7,7,7,7};
			var segment=new ArraySegment<int>(oversizedArray,1,collection.GetOverallLength());
			collection.CopyIntoSegment(segment);
			Compare(segment.Array,0,7,1,1,2,3,5,8,13,21,7);
		}

		private void Compare<T>(T[] data, int offset, params T[] expected)
		{
			for(int i=0; i<expected.Length; i++)
			{
				Assert.That(data[offset+i],Is.EqualTo(expected[i]));
			}
		}
	}
}
