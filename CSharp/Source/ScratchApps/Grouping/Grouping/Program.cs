using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Arrow.Execution;

namespace Grouping
{
	class Program
	{
		static void Main(string[] args)
		{
			IList<int> duplicateValues=null;

			var data=RandomRangeList(9000000,30);

			var elapsed=ExecutionTimer.Measure(TimerMode.Milliseconds,()=>
			{
				var duplicates=Duplicates(data);
				duplicateValues=duplicates.ToArray();
			});

			Console.WriteLine(elapsed);
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
