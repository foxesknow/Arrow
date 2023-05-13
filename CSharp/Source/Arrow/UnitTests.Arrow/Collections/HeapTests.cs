using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
    [TestFixture]
    public class HeapTests
    {
        [Test]
        public void AddToEmpty()
        {
            Heap<int> heap = new Heap<int>();
            heap.Add(10);

            Assert.That(heap.Count, Is.EqualTo(1));

            heap.AddRange(new int[] { 5, 3, 9, 12, 2 });
        }

        [Test]
        public void AddToSingle()
        {
            Heap<int> heap = new Heap<int>();
            heap.Add(10);
            heap.Add(12);

            Assert.That(heap.Count, Is.EqualTo(2));
        }

        [Test]
        public void BuildHeap()
        {
            for(int i = 1; i < 100; i++)
            {
                var data = new List<int>(Enumerable.Range(1, i));
                Heap<int> heap = new Heap<int>(data);

                int max = data[data.Count - 1];
                Assert.That(heap[0], Is.EqualTo(max));

                Assert.That(data.Count, Is.EqualTo(heap.Count));

                // Now work through the heap
                ValidateHeap(0, heap);
            }
        }

        [Test]
        public void Duplicates()
        {
            int[] data = { 8, 7, 3, 5, 8, 2, 1, 9, 12, 3, 1, 3, 2 };
            var heap = new Heap<int>(data);

            Assert.That(data.Length, Is.EqualTo(heap.Count));

            ValidateHeap(0, heap);
        }

        [Test]
        public void Extract()
        {
            for(int i = 1; i < 20; i++)
            {
                var data = new List<int>(Enumerable.Range(1, i));
                Heap<int> heap = new Heap<int>(data);

                int lastMax = int.MaxValue;

                while(heap.Count != 0)
                {
                    int max = heap.Extract();
                    Assert.That(max, Is.LessThanOrEqualTo(lastMax));

                    if(heap.Count > 1) ValidateHeap(0, heap);

                    lastMax = max;
                }
            }
        }

        [Test]
        public void Add()
        {
            Heap<int> heap = new Heap<int>();

            for(int i = 1; i < 100; i++)
            {
                heap.Add(i);
                Assert.That(heap[0], Is.EqualTo(i));

                // Now work through the heap
                ValidateHeap(0, heap);
            }
        }

        private void ValidateHeap(int index, Heap<int> heap)
        {
            int data = heap[index];

            int left;
            if(heap.TryGetLeft(index, out left))
            {
                Assert.That(data, Is.GreaterThanOrEqualTo(left));
                ValidateHeap(heap.Left(index), heap);
            }

            int right;
            if(heap.TryGetRight(index, out right))
            {
                Assert.That(data, Is.GreaterThanOrEqualTo(right));
                ValidateHeap(heap.Right(index), heap);
            }
        }
    }
}
