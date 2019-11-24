using System;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Text_File_Processing
{
    interface IReader
    {
        string GetCurrentWord { get; }
        bool GetNextWord();
    }

    class Reader : IReader
    {
        StreamReader stream;
        public string currentWord = string.Empty;
        int wordCount;
        public bool firstWord = false;

        public Reader(StreamReader stream)
        {
            this.stream = stream;
            wordCount = 0;
        }

        public string GetCurrentWord => currentWord;

        //Pick the next word as a current.
        public bool GetNextWord()
        {
            return (currentWord = ReadWord()) != null ? true : false;
        }


        //Does any character in a file is break, tabulator or space?
        //If so, return true; otherwise return false.
        public static bool IsWhiteSpace(char c) => (c == ' ' || c == '\n' || c == '\t' || c == '\r') ? true : false;


        public int WordCount()
        {
            while (GetNextWord())
                wordCount++;

            return wordCount;
        }

        char lastWhiteSpase = ' ';
        public int newLineCount = 0;
        public bool newParagraph = false;

        //Reads one char at a time.
        //And returns the list of the characters converted into a string.
        public string ReadWord()
        {
            if (stream.EndOfStream) return null;

            if (lastWhiteSpase == '\n')
            {
                newLineCount = 1;
            }
            string str = string.Empty;

            while (!stream.EndOfStream)
            {
                char chr = (char)stream.Read();

                if (IsWhiteSpace(chr))
                {
                    if (chr == '\n') newLineCount += 1;

                    if (str != string.Empty)
                    {
                        lastWhiteSpase = chr;
                        break;
                    }                                  //check if an end of the word 
                    else continue;                     //or continue to find non-whitespace
                }
                else if (firstWord == false)           //Ignore new line symbols before the first word.
                {
                    newLineCount = 0;  
                    firstWord = true;
                }

                if (newLineCount > 1)
                {
                    newParagraph = true;
                }

                newLineCount = 0;
                str += chr.ToString();
            }
            return str == string.Empty ? null : str;
        }

        //Put the words into a dictionary (alphabetically ordered).
        public SortedDictionary<string, int> GetFreq()
        {
            SortedDictionary<string, int> wordFreq = new SortedDictionary<string, int>();

            while (GetNextWord())
            {
                if (wordFreq.ContainsKey(currentWord))
                    wordFreq[currentWord] = (int)wordFreq[currentWord] + 1;
                else
                    wordFreq.Add(currentWord, 1);
            }

            return wordFreq;
        }
    }

    class FileReader
    {
        public static void ErrorHandling(string[] args, out int n, out bool highlights)
        {
            if (args.Length < 3)
                throw new ArgumentException("Argument Error");

            if (!int.TryParse(args[args.Length - 1], out n))
                throw new ArgumentException("Argument Error");

            if (args[0] == "--highlight-spaces") 
            {
                if (args.Length < 4)
                    throw new ArgumentException("Argument Error");

                highlights = true;
            }
            else
            {
                highlights = false;
            }
        }

        //Look through the arguments and find names and cound of the input files.
        public static void GetInputFiles(string[] args, bool highlights, out List<string> inputFileName)
        {
            inputFileName = new List<string>();
            
            for (int i = (highlights ? 1 : 0); i < args.Length - 2; i += 1)
            {
                if (File.Exists(args[i]))
                {
                    inputFileName.Add(args[i]);
                }
                else
                    continue;
            }
                    
        }


        static void Main(string[] args)
        {
            int n;
            bool highlights;
            List<string> inputFileName;
            bool nextFileExists;

            string file;

            try
            {
                ErrorHandling(args, out n, out highlights);
                GetInputFiles(args, highlights, out inputFileName);

                file = string.Empty;
                nextFileExists = false;

                StreamReader inFile = new StreamReader(inputFileName[0]);
                
                StreamWriter outFile = new StreamWriter(args[args.Length - 2]);

                TextAligner text = new TextAligner(inFile, int.Parse(args[args.Length - 1]), highlights);
                
                for (int i = 0; i < inputFileName.Count; i += 1 )
                {
                    nextFileExists = i < inputFileName.Count - 1;

                        StreamReader s = new StreamReader(inputFileName[i]);
                        text.SetReader(s , nextFileExists);

                        string line = null;
                        while (true)
                        {
                            line = text.NextLine();
                            if (line == null)
                                break;
                            outFile.WriteLine(line);
                        }
                    inFile.Close();
                } 
                outFile.Close();
            }

            
            catch (ArgumentException e)
            {
                WriteLine(e.Message);
            } 
        }
    }
}
