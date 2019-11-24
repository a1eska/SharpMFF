using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanAlgorithm
{
    interface INode
    {
        bool IsLeaf { get; }
        UInt64 ToUint64();
    }
    class Node : INode
    {
        public int? key = null;
        public ulong value;
        public int step = 0;
        public int height = 0;
        public Node left = null;
        public Node right = null;
        public Node parent = null;

        ///Node which represent a leaf.
        public Node(int key, ulong value)
        {
            this.key = key;
            this.value = value;
        }

        ///Node which represents an internal node.
        public Node(ulong value, int step, int height)
        {
            this.value = value;
            this.step = step;
            this.height = height;
        }

        public bool IsLeaf => this.height == 0;

        public UInt64 ToUint64()
        {
            ///set the bit zero
            UInt64 nodeCode = (UInt64)(IsLeaf ? 0x1 : 0x0);

            UInt64 value64;

            ///copy lower 55 bits
            value64 = value & 0xFFFFFFFFFFFFFF;
            ///binary shift by one to the left
            value64 <<= 1;

            nodeCode += value64;

            if (IsLeaf)
            {
                UInt64 k = (UInt64)key;
                k <<= 56;
                nodeCode += k;
            }

            return nodeCode;
        }
    }

    class NodeComparer : IComparer<Node>
    {
        private int ValueComparison(int? a, int? b)
        {
            if (a < b) return -1;
            else if (a > b) return 1;
            else return 0;
        }

        public int Compare(Node a, Node b)
        {
            if (a.value < b.value) return -1;
            else if (a.value > b.value) return 1;
            ///Both node a and b have the same value.
            else
            {
                ///Both node a and node b are leaves or both are not leaves.
                if (a.height == b.height)
                {
                    ///Lexicographical comparison by a key.
                    if (a.height == 0)
                    {
                        return ValueComparison(a.key, b.key);
                    }
                    ///Both are not leaves. Priority has the node which was made earlier.
                    else
                    {
                        return ValueComparison(a.step, b.step);
                    }
                }
                ///Node with the lower height has priority
                else
                {
                    return a.height < b.height ? -1 : 1;
                }
            }
        }
    }
}
