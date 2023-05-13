using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.GraphTheory.Build
{
	/// <summary>
	/// Useful class for generating build information
	/// </summary>
	/// <typeparam name="T">The type of the vertex to build</typeparam>
	public class BuildGenerator<T>
	{

        /// <summary>
        /// Analyses the build tasks and determines the order to build them
        /// </summary>
        /// <param name="buildTasks">The tasks to build</param>
        /// <returns>A list containing the target that must be built</returns>
        public List<T> SingleThreadedBuild(IList<IBuildDescription<T>> buildTasks)
        {
            DirectedGraph<T> graph = MakeGraph(buildTasks);
            return SingleThreadedBuild(graph);
        }

        /// <summary>
        /// Analyses a graph and determines the order to build the vertices
        /// </summary>
        /// <param name="graph">The graph to analyse</param>
        /// <returns>A build order list</returns>
        public List<T> SingleThreadedBuild(DirectedGraph<T> graph)
        {
            if(graph == null) throw new ArgumentNullException("graph");

            ValidateGraph(graph);

            List<VertexDescriptor<T>> topological = graph.TopologicalSort();
            List<T> targetsToBuild = VertexDescriptor<T>.Vertices(topological);

            return targetsToBuild;
        }

        /// <summary>
        /// Analyses the build tasks and determines the order to build them in parallel
        /// </summary>
        /// <param name="buildTasks">The tasks to build</param>
        /// <returns>A list containing the target that must be built</returns>
        public List<ParallelBuildItems<T>> MultiThreadedBuild(IList<IBuildDescription<T>> buildTasks)
        {
            DirectedGraph<T> graph = MakeGraph(buildTasks);
            return MultiThreadedBuild(graph);
        }

        /// <summary>
        /// Analyses the build tasks and determines the order to build them in parallel
        /// </summary>
        /// <param name="graph">The graph to analyse</param>
        /// <returns>A list containing the target that must be built</returns>
        public List<ParallelBuildItems<T>> MultiThreadedBuild(DirectedGraph<T> graph)
        {
            if(graph == null) throw new ArgumentNullException("graph");

            ValidateGraph(graph);

            ParallelRunShortestPathVisitor<T> visitor = new ParallelRunShortestPathVisitor<T>();
            ShortestPath<T> shortestPath = new ShortestPath<T>(graph, visitor);

            // Do a shortest path run for each possible start point
            VertexDescriptors<T> descriptors = graph.CalculateInOutDegree();
            foreach(VertexDescriptor<T> descriptor in descriptors)
            {
                if(descriptor.D == 0) shortestPath.Calculate(descriptor.Vertex);
            }

            // Now just sort by distance (D) to get the build order.
            // Items with the same distance can be build at the same time
            List<VertexDescriptor<T>> sortedDescriptors = new List<VertexDescriptor<T>>(shortestPath.Distances);
            sortedDescriptors.Sort(DistanceComparer);

            List<ParallelBuildItems<T>> parallelBuild = new List<ParallelBuildItems<T>>();
            ParallelBuildItems<T> active = new ParallelBuildItems<T>();
            parallelBuild.Add(active);

            double group = 0;
            foreach(VertexDescriptor<T> descriptor in sortedDescriptors)
            {
                if(descriptor.D == group + 1)
                {
                    active = new ParallelBuildItems<T>();
                    parallelBuild.Add(active);
                    group++;
                }

                if(descriptor.D == group)
                {
                    active.Add(descriptor.Vertex);
                }
            }

            return parallelBuild;
        }

        private DirectedGraph<T> MakeGraph(IList<IBuildDescription<T>> buildTasks)
        {
            DirectedGraph<T> graph = new DirectedGraph<T>();

            foreach(IBuildDescription<T> buildTask in buildTasks)
            {
                T target = buildTask.Target;

                foreach(T dependency in buildTask.Dependencies)
                {
                    if(target.Equals(dependency))
                    {
                        // It's a standalone build target
                        graph.AddVertex(target);
                    }
                    else
                    {
                        // The target has real dependencies
                        graph.Add(new Edge<T>(dependency, target));
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Makes sure there are no cycles in the graph
        /// </summary>
        /// <param name="graph"></param>
        private void ValidateGraph(DirectedGraph<T> graph)
        {
            CycleDetector<T> detector = new CycleDetector<T>();
            graph.DepthFirstSearch(detector);

            if(detector.HasCycle)
            {
                throw new BuildException("build tasks have a cycle");
            }
        }

        private int DistanceComparer(VertexDescriptor<T> lhs, VertexDescriptor<T> rhs)
        {
            return lhs.D.CompareTo(rhs.D);
        }
    }
}
