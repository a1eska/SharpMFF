using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Excel
{
    class Table
    {
        List<List<string>> table;

        Graph graph;
        Graph reverseGraph;


        string[] sep = { "+", "-", "*", "/", "=" };

        private bool IsEmpty(string line) => line == string.Empty || line == null;

        public Table(StreamReader inFile)
        {
            this.table = new List<List<string>>();

            CreateGraph();

            while(!inFile.EndOfStream)
            {
                string line = inFile.ReadLine();
                List<string> row = new List<string>(line.Split(' ', StringSplitOptions.RemoveEmptyEntries));

                table.Add(row);

                for (int i = 0; i < row.Count; i += 1)
                {
                    if (IsExpression(row[i]))
                    {
                        var u = new Tuple<int, int>(table.Count - 1, i);
                        CreateDependences(row[i], u);
                    }
                }
            }
        }

        public void CreateDependences(string s, Tuple<int, int> u)
        {
            if (!graph.ContainsVertex(u))
            {
                graph.AddVertex(u);
                reverseGraph.AddVertex(u);
            }

            string[] depend = s.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            foreach (string d in depend)
            {
                Tuple<int, int> v = StringToTuple(d);

                if (!graph.ContainsVertex(v))
                {
                    graph.AddVertex(v);
                    reverseGraph.AddVertex(v);
                }

                graph.AddEdge(v, u);
                reverseGraph.AddEdge(u, v);
            }
        }

        public void CreateGraph()
        {
            graph = new Graph();
            reverseGraph = new Graph();
        }

        public Tuple<int, int> StringToTuple(string s)
        {
            Regex regExp = new Regex(@"([A-Z]+)(\d+)");
            Match result = regExp.Match(s);

            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;

            return new Tuple<int, int>(int.Parse(numberPart) - 1, ConvertBase(alphaPart));
        }

        public int ConvertBase(string s)
        {
            int power = 1;
            int result = 0;

            for (int i = 0; i < s.Length; i += 1)
            {
                result += (s[i] - 'A') * power;
                power *= 26;
            }
            return result;
        }

        public bool IsExpression(string s) => s[0] == '=';
    }
}
