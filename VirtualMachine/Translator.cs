using System;
using System.Collections.Generic;
using System.IO;

namespace VirtualMachine
{
    internal class Parser
    {
        private static string LINECOMMENT = "//";
        public static List<string> Parse(string[] input)
        {
            List<string> ret = new List<string>();
            foreach(string line in input)
            {
                string[] linePieces = line.Split(new string[] {" "},StringSplitOptions.RemoveEmptyEntries);
                foreach(string linePiece in linePieces)
                {
                    if (linePiece.Contains(LINECOMMENT))
                    {
                        break;
                    }
                    else
                    {
                        ret.Add(linePiece);
                    }
                }
            }
            return ret;
        }
    }
    internal class Translator
    {
        string[] fileContents;
        //Stack<string> _stack;
        List<string> parsedInput;
        List<string> hackCode;
        internal void LoadFile(string fileName)
        {
            fileContents = File.ReadAllLines(fileName);
        }

        internal void Parse()
        {
            parsedInput = Parser.Parse(fileContents);
        }
        internal void TranslateToHackAssem()
        {
            hackCode = new List<string>();
            TranslateToHackAssem(0);
        }
        private int TranslateToHackAssem(int index)
        {
            if (index >= parsedInput.Count)
            {
                return 0;
            }

            switch (parsedInput[index])
            {
                case "push":
                    //Keep track of how many tokens we are consuming
                    //int next = TranslateToHackAssem(index + 1);
                    int next;
                    if (parsedInput[index + 1] == "constant")
                    {
                        hackCode.Add("@" + parsedInput[index + 2]);
                        hackCode.Add("D=A");
                        hackCode.Add("@SP");
                        hackCode.Add("A=M");
                        hackCode.Add("M=D");
                        hackCode.Add("@SP");
                        hackCode.Add("M=M+1");
                        next = 2;
                    }
                    else
                    {
                        next = 1;
                    }
                    //Goto the next token that hasn't been consumed.
                    return TranslateToHackAssem(index + next + 1);
                case "add":
                    AddArithmeticHeader();
                    //Add specific
                    hackCode.Add("D=D+M");
                    AddArithmeticFooter();
                    return TranslateToHackAssem(index + 1);
                case "eq":
                    AddArithmeticHeader();

                    AddArithmeticFooter();
                    return TranslateToHackAssem(index + 1);
                default:
                    return 0;
            }
        }
        public void AddArithmeticHeader()
        {
            hackCode.Add("@SP");
            hackCode.Add("M=M-1");
            hackCode.Add("A=M-1");
            hackCode.Add("D=M");
            hackCode.Add("A=A+1");
        }
        public void AddArithmeticFooter()
        {
            hackCode.Add("A=A-1");
            hackCode.Add("M=D");
        }
        public void Save(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.WriteAllLines(fileName,hackCode);
        }
    }
}