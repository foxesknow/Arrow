using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using NUnit;
using NUnit.Framework;

using Arrow.Execution;

namespace Grouping
{
	[TestFixture]
	public class GroupingTests
	{
		[TestCase(9000000,30)]
		public void Duplicates_Array(int numberOfItems, int maxValue)
		{
			var data=RandomRangeList(numberOfItems,maxValue);
			IList<int> duplicateValues=null;

			var elapsedTime=ExecutionTimer.Measure(TimerMode.Milliseconds,()=>
			{
				var duplicates=Duplicates(data);
				duplicateValues=duplicates.ToArray();
			});

			CheckUniqueness(duplicateValues);

			Console.WriteLine("Duplicates_Array({0}, {1}) found {2} in {3}",numberOfItems,maxValue,duplicateValues.Count,elapsedTime);
		}

		[TestCase(9000000,30)]
		public void DuplicatesLinq_Array(int numberOfItems, int maxValue)
		{
			var data=RandomRangeList(numberOfItems,maxValue);
			IList<int> duplicateValues=null;

			var elapsedTime=ExecutionTimer.Measure(TimerMode.Milliseconds,()=>
			{
				var duplicates=LinqDuplicates(data);
				duplicateValues=duplicates.ToArray();
			});

			CheckUniqueness(duplicateValues);

			Console.WriteLine("DuplicatesLinq_Array({0}, {1}) found {2} in {3}",numberOfItems,maxValue,duplicateValues.Count,elapsedTime);
		}

		[TestCase(9000000,30)]
		public void Duplicates_Enumerable(int numberOfItems, int maxValue)
		{
			var data=RandomRange(numberOfItems,maxValue);
			IList<int> duplicateValues=null;

			var elapsedTime=ExecutionTimer.Measure(TimerMode.Milliseconds,()=>
			{
				var duplicates=Duplicates(data);
				duplicateValues=duplicates.ToArray();
			});

			CheckUniqueness(duplicateValues);

			Console.WriteLine("Duplicates_Enumerable({0}, {1}) found {2} in {3}",numberOfItems,maxValue,duplicateValues.Count,elapsedTime);
		}

		[TestCase(9000000,30)]
		public void DuplicatesLinq_Enumerable(int numberOfItems, int maxValue)
		{
			var data=RandomRange(numberOfItems,maxValue);
			IList<int> duplicateValues=null;

			var elapsedTime=ExecutionTimer.Measure(TimerMode.Milliseconds,()=>
			{
				var duplicates=LinqDuplicates(data);
				duplicateValues=duplicates.ToArray();
			});

			CheckUniqueness(duplicateValues);

			Console.WriteLine("DuplicatesLinq_Enumerable({0}, {1}) found {2} in {3}",numberOfItems,maxValue,duplicateValues.Count,elapsedTime);
		}

		private void CheckUniqueness(IList<int> values)
		{
			var distinct=values.Distinct();

			Assert.That(values.Count,Is.EqualTo(distinct.Count()));
		}

		static IEnumerable<T> Duplicates<T>(IEnumerable<T> items)
		{
			var state=new Dictionary<T,int>();

			foreach(var item in items)
			{
				int flag;
				state.TryGetValue(item,out flag);

				switch(flag)
				{
					case 0:
						// We've never seen it before, so just record it
						state.Add(item,1);
						break;

					case 1:
						// We've seen if before, but only once
						yield return item;
						state[item]=2;
						break;

					default:
						// We've seen it more than once, and already told the caller, so don't tell them again
						break;
				}
			}			
		}

		static IEnumerable<T> LinqDuplicates<T>(IEnumerable<T> items)
		{
			return	items.GroupBy(s=>s)				// Group by the item
					.Where(group=>group.Count()>1)	// Restrict to cases where there's a duplicate
					.Select(group=>group.Key);		// The key in the item that's duplicated
		}

		static IEnumerable<int> RandomRange(int numberOfItems, int maxValue)
		{
			var random=new Random();

			for(int i=0; i<numberOfItems; i++)
			{
				yield return random.Next(maxValue);
			}
		}

		static IList<int> RandomRangeList(int numberOfItems, int maxValue)
		{
			var items=new int[numberOfItems];
			var random=new Random();

			for(int i=0; i<numberOfItems; i++)
			{
				items[i]=random.Next(maxValue);
			}

			return items;
		}
	}
}
