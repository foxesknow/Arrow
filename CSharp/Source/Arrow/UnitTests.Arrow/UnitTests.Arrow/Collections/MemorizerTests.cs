using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class MemorizerTests
	{
		[Test]
		public void Test_Lookup()
		{
			Func<int,int> plusOne=x=>
			{
				return x+1;
			};
			
			Memorizer<int,int> memo=new Memorizer<int,int>(plusOne);
			
			// The first lookup will call plusOne
			Assert.IsTrue(memo.Lookup(1)==2);
			Assert.IsTrue(memo.Count==1);
			
			// The second lookup should come from the memorizer, so nothing will change
			Assert.IsTrue(memo.Lookup(1)==2);
			Assert.IsTrue(memo.Count==1);
			
			Assert.IsTrue(memo.Lookup(2)==3);
			Assert.IsTrue(memo.Count==2);
		}
		
		[Test]
		public void Test_CallPattern()
		{
			bool called=false;
			
			Func<int,int> plusOne=x=>
			{
				called=true;
				return x+1;
			};
			
			Memorizer<int,int> memo=new Memorizer<int,int>(plusOne);
			
			// The first lookup will call plusOne
			Assert.IsTrue(memo.Lookup(1)==2);
			Assert.IsTrue(memo.Count==1);
			Assert.IsTrue(called);
			
			// The second lookup should come from the memorizer, so nothing will change
			called=false;
			Assert.IsTrue(memo.Lookup(1)==2);
			Assert.IsTrue(memo.Count==1);
			Assert.IsFalse(called);
			
			called=true;
			Assert.IsTrue(memo.Lookup(2)==3);
			Assert.IsTrue(memo.Count==2);
			Assert.IsTrue(called);
			
			memo.Clear();
			called=false;
			Assert.IsTrue(memo.Lookup(2)==3);
			Assert.IsTrue(memo.Count==1);
			Assert.IsTrue(called);
		}
		
		[Test]
		public void Test_Recursive()
		{
			Func<long,long> fib=null;
			
			fib=Memorizer<long,long>.Memorize(n=>
			{
				if(n==0) return 0;
				if(n==1) return 1;
				
				return fib(n-1)+fib(n-2);
			});
			
			Assert.IsTrue(fib(0)==0);
			Assert.IsTrue(fib(1)==1);
			Assert.IsTrue(fib(2)==1);
			Assert.IsTrue(fib(3)==2);
			Assert.IsTrue(fib(4)==3);
			Assert.IsTrue(fib(5)==5);
			Assert.IsTrue(fib(6)==8);
		}	
		
	}
}
