using System;
using System.Collections.Generic;
using System.Text;

using Arrow.Collections;
using Arrow.GraphTheory;

using NUnit.Framework;


namespace UnitTests.Arrow.GraphTheory
{
	[TestFixture]
	public class EqualityTests
	{
        [Test]
        public void CaseImportant()
        {
            DirectedGraph<string> graph = new DirectedGraph<string>();
            graph.Add("socks", "shoes");
            graph.Add("Socks", "Shoes");

            Assert.That(graph.AllEdges().Count, Is.EqualTo(2));
        }

        [Test]
        public void IgnoreCase()
        {
            DirectedGraph<string> graph = new DirectedGraph<string>(StringComparer.OrdinalIgnoreCase);
            graph.Add("socks", "shoes");
            graph.Add("Socks", "Shoes");

            Assert.That(graph.AllEdges().Count, Is.EqualTo(1));
        }
    }
}
