using System;
using System.IO;

namespace HuffmanAlgorithm
{
    class Program
    {
        public static void ErrorHandling(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("Argument Error");
        }

        static void Main(string[] args)
        {
            try
            {
                ErrorHandling(args);

                string outFileName = args[0] + ".huff";
                BinaryWriter output = new BinaryWriter(File.Open(outFileName, FileMode.Create));

                Encoder coder = new Encoder(args[0]);
                coder.Encode(output);
                output.Close();
            }
            catch (IOException)
            {
                Console.Write("File Error");
            }

            catch (ArgumentException e)
            {
                Console.Write(e.Message);
            }
        }
    }
}
