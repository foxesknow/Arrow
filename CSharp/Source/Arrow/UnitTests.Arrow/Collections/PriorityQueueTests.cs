using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class PriorityQueueTests : PriorityQueueTestsBase
	{
		protected override IPriorityQueue<Priority,string> CreateStringQueue()
		{
			return new PriorityQueue<Priority,string>();
		}

		protected override IPriorityQueue<Priority,int> CreateIntegerQueue()
		{
			return new PriorityQueue<Priority,int>();
		}
	}
}
