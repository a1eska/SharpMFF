using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace HuffmanAlgorithm
{
    class Encoder
    {
        HuffmanTree tree;
        byte[] header = { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };

        LinkedList<bool> queue = new LinkedList<bool>();
        bool[] slidingWindow = new bool[8];

        List<UInt64> treeCode;
        LinkedList<bool>[] pathTable;
        string fileName;
        
        public Encoder(string fileName)
        {
            this.fileName = fileName;
            tree = new HuffmanTree();

            tree.CreateTree(fileName);

            treeCode = tree.GetTreeCode();
            pathTable = tree.GetPathTable();
        }

        byte WindowToByte()
        {
            byte sum = 0;
            byte power = 1;
            for (int i = 0; i < 8; i++)
            {
                if (slidingWindow[i])
                    sum += (byte) (power * 1);
                power *= 2;
            }
            return sum;
        }

        public void Encode(BinaryWriter writer)
        {
            writer.Write(header);
            EncodeTree(writer);
            writer.Write((UInt64)0x00);
            EncodeContent(fileName, writer);  
        }

        public void EncodeTree(BinaryWriter writer)
        {
            foreach(UInt64 i in treeCode)
            {
                writer.Write(i);
            }
        }

        private void FillWindow()
        {
            for (int i = 0; i < 8; i += 1)
            {
                slidingWindow[i] = queue.First.Value;
                queue.RemoveFirst();
            }
        }

        public void EncodeContent(string fileName, BinaryWriter writer)
        {
            var file = File.OpenRead(fileName);
            int key = file.ReadByte();

            if (key == -1) return; //EOF

            while (key != -1)
            {
                foreach (bool b in pathTable[key])
                {
                    queue.AddLast(b);
                }

                while (queue.Count >= 8)
                {
                    FillWindow();
                    /*
                    for (int i = 0; i < 8; i += 1)
                    {
                        slidingWindow[i] = queue.First.Value;
                        queue.RemoveFirst();
                    } */

                    writer.Write(WindowToByte());
                }

                key = file.ReadByte();
            }

            if (queue.Count > 0)
            {
                while (queue.Count != 8)
                {
                    queue.AddLast(false);
                }

                for (int i = 0; i < 8; i += 1)
                {
                    slidingWindow[i] = queue.First.Value;
                    queue.RemoveFirst();
                }

                writer.Write(WindowToByte());
            }

            file.Close();
        }
    }
}
