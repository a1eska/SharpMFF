using System;
using System.Collections.Generic;
using System.IO;

namespace HuffmanAlgorithm
{
    interface ISet
    {
        bool Add(Node n);
        bool Remove(Node n);
        bool Contains(Node n);
        bool IsEmpty { get; }
        bool SingleResidue { get; }
        Node Min { get; }
    }

    class Forest : ISet
    {
        private SortedSet<Node> forestQueue;

        public Forest(ulong[] leaves)
        {
            this.forestQueue = new SortedSet<Node>(new NodeComparer());
            
            for(int i = 0; i < leaves.Length; i += 1)
            {
                if (leaves[i] > 0)
                {
                    forestQueue.Add(new Node((byte)i, leaves[i]));
                }
            }
        }

        //Get the minimum value in the forest as defined in comparer.
        public Node Min => forestQueue.Min;

        public bool Add(Node n)
        {
            return forestQueue.Add(n);
        }
        
        public bool Remove(Node n)
        {
            return forestQueue.Remove(n);
        }

        public bool Contains(Node n)
        {
            return forestQueue.Contains(n);
        }

        public bool IsEmpty => forestQueue.Count == 0;

        public bool SingleResidue => forestQueue.Count == 1;
    }

    class HuffmanTree 
    {
        public Node root;
        ulong[] charFrequency;
        Forest forest;
        LinkedList<bool>[] pathTable = new LinkedList<bool>[256];
        LinkedList<bool> path = new LinkedList<bool>();

        public HuffmanTree()
        {
            this.root = null;
        }

        /// <returns>Wtrite the huffman tree as a string</returns>
        public override string ToString()
        {
            return PreOrderString(this.root);
        }

        public void LeafWriter(Node n, TextWriter writer)
        {
            writer.Write('*');
            writer.Write(n.key);
            writer.Write(':');
            writer.Write(n.value);
            writer.Write(' ');
        }

        public void NodeWriter(Node n, TextWriter writer)
        {
            writer.Write(n.value);
            writer.Write(' ');
        }

        public void WriteTree()
        {
            PreOrder(this.root, Console.Out);
        }

        /// <summary>Huffman tree preorder</summary>
        public void PreOrder(Node n, TextWriter writer)
        {
            if (n == null)
            {
                writer.Write(string.Empty);
                return;
            }

            if (n.height == 0)
            {
                LeafWriter(n, writer);
            }
            else
            {
                NodeWriter(n, writer);
            }

            PreOrder(n.left, writer);
            PreOrder(n.right, writer);
        }

        List<UInt64> treeCode;

        public List<UInt64> GetTreeCode()
        {
            treeCode = new List<UInt64>();

            PreOrderTreeCode(this.root);

            return treeCode;
        }

        void PreOrderTreeCode(Node n)
        {
            if (n.IsLeaf)
            {
                treeCode.Add(n.ToUint64());
                return;
            }

            treeCode.Add(n.ToUint64());
            PreOrderTreeCode(n.left);
            PreOrderTreeCode(n.right);
        }

        /// <returns>table of the path codes of the leaves</returns>
        public LinkedList<bool>[] GetPathTable()
        {
            PathTable(this.root);
            return pathTable;
        }

        /// <summary>Create path codes for each leaf</summary>
        private void PathTable(Node n)
        {
            if (n.IsLeaf)
            {
                int i = (int)n.key;
                pathTable[i] = new LinkedList<bool>();

                foreach (bool b in path)
                    pathTable[i].AddLast(b);
                return;
            }

            path.AddLast(false);
            PathTable(n.left);
            path.RemoveLast();

            path.AddLast(true);
            PathTable(n.right);
            path.RemoveLast();
        }

        public string PreOrderString(Node n)
        {
            if (n == null) return string.Empty;

            string s = n.height == 0 ? ($"*{n.key}:{n.value}") : $"{n.value}";
            
            string left = PreOrderString(n.left); 
            string right = PreOrderString(n.right); 

            return s + (left != string.Empty? " " : string.Empty) + left + (right != string.Empty ? " " : string.Empty) + right;
        }

        /// <returns>true if the end of stream</returns>
        public bool EndOfStream(int n)
        {
            return (n == -1);
        }

        /// <summary>Read the file char by char in decimal and count their frequency</summary>
        public ulong[] GetFrequency(string fileName)
        {
            var file = File.OpenRead(fileName);
            int key = file.ReadByte();

            if (EndOfStream(key)) return null;
            
            charFrequency = new ulong[256];

            while (!EndOfStream(key))
            {
                charFrequency[key] += 1;

                key = file.ReadByte();
            }

            file.Close();

            return charFrequency;
        }

        public void CreateTree(string fileName)
        {
            GetFrequency(fileName);

            forest = new Forest(charFrequency);
            
            while (!forest.SingleResidue)
            {
                MakeStep();
            }

            this.root = forest.Min;
        }

        public Node GetRoot => root;

        public int step = 0;

        /// <summary>Remove two nodes from the forest. Make new one with the sum of the values of two removed nodes and add it to the forest</summary>
        private void MakeStep()
        {
            Node a = forest.Min;
            forest.Remove(a);
            Node b = forest.Min;
            forest.Remove(b);
            
            Node newNode = MergeNodes(a, b);
            newNode.step = step;

            forest.Add(newNode); 
        }

        /// <summary>Make new node with the value which is a sum of the values of the input nodes</summary>
        private Node MergeNodes(Node a, Node b)
        {
            Node n = new Node(a.value + b.value, step, b.height + 1);

            a.parent = n;
            b.parent = n;

            step += 1;

            n.left = a;
            n.right = b;

            return n;
        }
    }
}
