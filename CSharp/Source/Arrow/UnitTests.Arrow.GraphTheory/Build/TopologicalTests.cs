using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory;

using NUnit.Framework;


namespace UnitTests.Arrow.GraphTheory.Build
{
    [TestFixture]
    public class TopologicalTests : GraphTestBase
    {
        [Test]
        public void Sort()
        {
            DirectedGraph<string> graph = MakeDressGraph();
            List<string> items = VertexDescriptor<string>.Vertices(graph.TopologicalSort());

            Assert.IsNotNull(items);
            Assert.That(items.Count, Is.EqualTo(graph.VertexCount));

            // The jacket goes on after the belt
            Assert.That(items.IndexOf("jacket"), Is.GreaterThan(items.IndexOf("belt")));

            // The shoes goes on after the pants
            Assert.That(items.IndexOf("shoes"), Is.GreaterThan(items.IndexOf("pants")));
        }
    }
}
