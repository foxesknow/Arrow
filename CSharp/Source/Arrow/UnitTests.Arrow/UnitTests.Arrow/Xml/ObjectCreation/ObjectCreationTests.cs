using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Xml.ObjectCreation;

using UnitTests.Arrow.Support;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Xml.ObjectCreation
{
	[TestFixture]
	public class ObjectCreationTests
	{
		[Test]
		public void CreateList()
		{
			var doc=ResourceLoader.LoadXml("ObjectCreationTests.xml");
			var listNode=doc.SelectSingleNode("root/List");
			
			object obj=XmlCreation.Create<object>(listNode);
			Assert.IsNotNull(obj);
			Assert.That(obj,Is.TypeOf(typeof(List<int>)));
		}
		
		[Test]
		public void Tuple2()
		{
			var doc=ResourceLoader.LoadXml("ObjectCreationTests.xml");
			var tupleNode=doc.SelectSingleNode("root/Tuple");
			
			object obj=XmlCreation.Create<object>(tupleNode);
			Assert.IsNotNull(obj);
			Assert.That(obj,Is.TypeOf(typeof(TestTuple<string,int>)));
			
			var t=(TestTuple<string,int>)obj;
			
			Assert.That(t.First,Is.EqualTo("Hurley"));
			Assert.That(t.Second,Is.EqualTo(39));
		}
		
		[Test]
		public void Nested()
		{
			var doc=ResourceLoader.LoadXml("ObjectCreationTests.xml");
			var nestedNode=doc.SelectSingleNode("root/Nested");
			
			object obj=XmlCreation.Create<object>(nestedNode);
			Assert.IsNotNull(obj);
			Assert.That(obj,Is.TypeOf(typeof(Dictionary<int,TestTuple<string,int?>>)));			
		}
		
		[Test]
		[ExpectedException(typeof(XmlCreationException))]
		public void NotEnoughGenerics()
		{
			var doc=ResourceLoader.LoadXml("ObjectCreationTests.xml");
			var node=doc.SelectSingleNode("root/NotEnough");
			
			object obj=XmlCreation.Create<object>(node);
			Assert.Fail(); // We should never get here
		}
	}
}
