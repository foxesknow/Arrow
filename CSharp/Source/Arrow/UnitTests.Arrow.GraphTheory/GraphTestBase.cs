using System;
using System.Collections.Generic;
using System.Text;

using Arrow.GraphTheory;

namespace UnitTests.Arrow.GraphTheory
{
	public class GraphTestBase
	{
		protected Graph<int> MakeNumericGraph()
		{
			DirectedGraph<int> graph=new DirectedGraph<int>();
			graph.Add(7,11);
			graph.Add(7,8);
			graph.Add(5,11);
			graph.Add(3,8);
			graph.Add(3,10);
			graph.Add(11,2);
			graph.Add(11,9);
			graph.Add(11,10);
			graph.Add(8,9);
			
			return graph;
		}
		
		protected DirectedGraph<string> MakeDressGraph()
		{
			DirectedGraph<string> graph=new DirectedGraph<string>();
			
			graph.Add("socks","shoes");
			graph.Add("undershorts","shoes");
			graph.Add("undershorts","pants");
			graph.Add("pants","shoes");
			graph.Add("pants","belt");
			graph.Add("shirt","belt");
			graph.Add("shirt","tie");
			graph.Add("belt","jacket");
			graph.Add("tie","jacket");
			graph.Add("watch","watch");
			
			return graph;
		}
	}
}
