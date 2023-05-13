using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory;

using NUnit.Framework;


namespace UnitTests.Arrow.GraphTheory
{
	[TestFixture]
	public class EdgeTests : GraphTestBase
	{
        [Test]
        public void UV()
        {
            Edge<string> edge = new Edge<string>("Jack", "Sawyer");
            Assert.That(edge.From, Is.Not.EqualTo(edge.To));
        }

        [Test]
        public void Equality()
        {
            Edge<string> edge1 = new Edge<string>("Jack", "Sawyer");
            Assert.That(edge1, Is.EqualTo(edge1));

            Edge<string> edge2 = new Edge<string>("Jack", "Sawyer");
            Assert.That(edge1, Is.EqualTo(edge2));

            Edge<string> edge3 = new Edge<string>("Ben", "Sawyer");
            Assert.That(edge1, Is.Not.EqualTo(edge3));
        }

        [Test]
        public void SearchForEdge()
        {
            Graph<int> graph = MakeNumericGraph();

            Assert.IsTrue(graph.ContainsEdge(new Edge<int>(7, 8)));
            Assert.IsFalse(graph.ContainsEdge(new Edge<int>(8, 7)));

            IList<Edge<int>> edges = graph.EdgesConnectedTo(11);
            Assert.IsNotNull(edges);
            Assert.That(edges, Has.Count.EqualTo(3));

            Assert.That(edges, Has.Member(graph.NewEdge(11, 2)));
            Assert.That(edges, Has.Member(graph.NewEdge(11, 9)));
            Assert.That(edges, Has.Member(graph.NewEdge(11, 10)));
        }
    }
}
