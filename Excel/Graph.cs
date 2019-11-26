using System;
using System.Collections.Generic;
using System.Text;

namespace Excel
{
    interface IGraph
    {
        void AddEdge(Tuple<int, int> v, Tuple<int, int> u);
        void AddVertex(Tuple<int, int> vertex);
        bool ContainsVertex(Tuple<int, int> vertex);
    }

    class Graph : IGraph
    {
        private Dictionary<Tuple<int, int>, List<Tuple<int, int>>> graph;

        private Dictionary<Tuple<int, int>, int> inDeg;
        private Dictionary<Tuple<int, int>, int> outDeg;

        public Graph()
        {
            this.graph = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>();
            this.inDeg = new Dictionary<Tuple<int, int>, int>();
            this.outDeg = new Dictionary<Tuple<int, int>, int>();
        }

        public void AddEdge(Tuple<int, int> v, Tuple<int, int> u)
        {
            graph[v].Add(u);

            outDeg[v] += 1;
            inDeg[u] += 1;
        }

        public void AddVertex(Tuple<int, int> vertex)
        {
            graph.Add(vertex, new List<Tuple<int, int>>());
            inDeg.Add(vertex, 0);
            outDeg.Add(vertex, 0);
        }

        public bool ContainsVertex(Tuple<int, int> vertex) => graph.ContainsKey(vertex);
    }
}
