using System;
using System.Collections.Generic;
using System.IO;

namespace Text_File_Processing
{

    class TextAligner 
    {
        Reader stream;
        int lineLength;
        string current;
        bool highlights;
        bool existsNext;

        List<string> lastWords;

        public TextAligner(StreamReader stream, int lineLength, bool highlights)
        {
            this.stream = new Reader(stream);
            this.lineLength = lineLength;
            this.highlights = highlights;

            lastWords = null;
        }

        //Set streamer
        public void SetReader(StreamReader stream, bool existsNext)
        {
            this.stream = new Reader(stream);
            this.existsNext = existsNext;
        }

        //Get the list of spaces.
        private List<string> GetSpaces(List<int> spaceCount)
        {
            List<string> spaces = new List<string>();

            for (int i = 0; i < spaceCount.Count; i++)
            {
                string s = new string(highlights ? '.' : ' ', spaceCount[i]);
                spaces.Add(s);
            }
            return spaces;
        }

        //Concatenate two lists into one string.
        private string MergeLists(List<string> word,
                                  List<string> space)
        {
            string line = string.Empty;

            for (int i = 0; i < word.Count - 1; i++)
                line += word[i] + space[i];

            if ((word.Count - 1) != 0)
                line += word[word.Count - 1];

            return line;
        }


        public string GetLine(List<string> words, int stringLength)
        {
            if (words.Count == 1) return words[0];

            List<int> spaceCount = new List<int>();
            int j = (lineLength - stringLength) % (words.Count - 1);

            for (int i = 0; i < (words.Count - 1); i++)
            {
                if (j == 0)
                    spaceCount.Add((lineLength - stringLength) / (words.Count - 1));
                else
                {
                    spaceCount.Add(((lineLength - stringLength) / (words.Count - 1)) + 1);
                    j--;
                }
            }
            return MergeLists(words, GetSpaces(spaceCount));
        }

        private string EndOfParagraph(List<string> words)
        {
            string s = string.Empty;

            for (int i = 0; i < words.Count - 1; i += 1)
                s += words[i] + (highlights ? "." :" ");
            s += words[words.Count - 1];

            return s;
        }

        string extraWord = string.Empty;
        //string lastWords;

        public string NextLine()
        {
            if (stream.currentWord == null) return null;

            List<string> wordList;
            int stringLength = 0;

            if (lastWords != null)
            {
                wordList = new List<string>(lastWords);

                for (int i = 0; i < lastWords.Count ; i += 1)
                    stringLength += lastWords[i].Length;

                lastWords = null;
            }
            else wordList = new List<string>();

            string newWord;

            //Initialization of the first word.
            if ((wordList == null) && (stream.currentWord == string.Empty) && (stream.firstWord != true))
            {
                newWord = stream.ReadWord();
                stream.currentWord = newWord;

                if (newWord != null)
                {
                    wordList.Add(newWord);
                    stringLength += newWord.Length;
                }
                else return null;
            } 

            //if there is a word left, which did not fit in the previous line
            if (extraWord != string.Empty)
            {
                wordList.Add(extraWord);
                stringLength += extraWord.Length;

                extraWord = string.Empty;
            }


            while (stream.GetNextWord()) 
            {
                newWord = stream.currentWord; 
                
                //if the end of the paragraph was found,
                //the last line should be reterned without alignment.
                if ((stream.newParagraph) && (newWord != string.Empty))
                {
                    stream.newParagraph = false;
                    if (wordList.Count > 1)   
                    {
                        extraWord = newWord;
                        return EndOfParagraph(wordList) + (highlights ? "<-" : "") + "\n" + (highlights ? "<-" : "");   //Separate words in the last line of the paragraph with single spaces.
                    }
                    else if (wordList.Count == 1)
                    {
                        extraWord = newWord;
                        return wordList[0] + (highlights ? "<-" : "") + "\n" + (highlights ? "<-" : "");
                    }
                    stringLength = 0;
                    wordList.Clear();
                    newWord = string.Empty;
                }

                //try to fit the word into the line
                if (newWord != string.Empty && ((stringLength + newWord.Length + wordList.Count) <= lineLength))
                {
                    wordList.Add(newWord);
                    stringLength += newWord.Length;
                }
                else if (newWord != string.Empty)
                {
                    extraWord = newWord;
                    break;
                }
            }
            
            //If the word is longer than a line width 
            if ((wordList.Count == 0) && (extraWord != string.Empty) && (extraWord.Length >= lineLength))
            {
                string s = extraWord;
                extraWord = string.Empty;

                return s + (highlights ? "<-" : "");
            }

            
            //Last line if the end of file. т_т
            if (stream.currentWord == null) 
            {
                if ((!existsNext) && (stream.currentWord == null)) return EndOfParagraph(wordList) + (highlights ? "<-" : "");
                
                lastWords = new List<string>(wordList);
                return null;
            }
            return GetLine(wordList, stringLength) + (highlights ? "<-" : "");
        }
    }
}