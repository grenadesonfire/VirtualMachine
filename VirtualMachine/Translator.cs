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
        public string[] fileContents
        {
            get; private set;
        }
        //Stack<string> _stack;
        List<string> parsedInput;
        public List<string> hackCode { get; private set; }
        int _arithLabelCnt = 0;
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
            _arithLabelCnt = 0;
            TranslateToHackAssem(0);
        }
        private void TranslateToHackAssem(int index)
        {
            if (index >= parsedInput.Count)
            {
                return;
            }
            while (index < parsedInput.Count)
            {
                switch (parsedInput[index])
                {
                    case "push":
                        if (parsedInput[index + 1] == "constant")
                        {
                            hackCode.Add("@" + parsedInput[index + 2]);
                            hackCode.Add("D=A");
                            Push();
                        }
                        else if (parsedInput[index + 1] == "local")
                        {
                            hackCode.Add("@LCL");
                            PushPointer(index);
                        }
                        else if (parsedInput[index + 1] == "argument")
                        {
                            hackCode.Add("@ARG");
                            PushPointer(index);
                        }
                        else if(parsedInput[index+1] == "this")
                        {
                            hackCode.Add("@THIS");
                            PushPointer(index);
                        }
                        else if(parsedInput[index+1] == "that")
                        {
                            hackCode.Add("@THAT");
                            PushPointer(index);
                        }
                        else if(parsedInput[index+1] == "temp")
                        {
                            hackCode.Add("@" + (5 + int.Parse(parsedInput[index + 2])));
                            PushPointer(index);
                        }
                        //Goto the next token that hasn't been consumed.
                        //return TranslateToHackAssem(index + next + 1);
                        index += 2;
                        break;
                    case "pop":
                        if (parsedInput[index + 1] == "local")
                        {
                            hackCode.Add("@LCL");
                        }
                        else if (parsedInput[index + 1] == "argument")
                        {
                            hackCode.Add("@ARG");
                        }
                        else if (parsedInput[index + 1] == "this")
                        {
                            hackCode.Add("@THIS");
                        }
                        else if (parsedInput[index + 1] == "that")
                        {
                            hackCode.Add("@THAT");
                        }
                        else if (parsedInput[index + 1] == "temp")
                        {
                            hackCode.Add("@" + (5 + int.Parse(parsedInput[index + 2])));
                        }
                        Pop();
                        //Goto the next token that hasn't been consumed.
                        //return TranslateToHackAssem(index + next + 1);
                        index += 2;
                        break;
                    case "add":
                        AddArithmeticHeader();
                        //Add specific
                        hackCode.Add("D=D+M");
                        AddArithmeticFooter();
                        //return TranslateToHackAssem(index + 1);
                        break;
                    case "sub":
                        AddArithmeticHeader();
                        hackCode.Add("D=D-M");
                        AddArithmeticFooter();
                        //return TranslateToHackAssem(index + 1);
                        break;
                    case "neg":
                        AddArithmeticHeader(1);
                        hackCode.Add("D=-D");
                        AddArithmeticFooter(1);
                        //return TranslateToHackAssem(index + 1);
                        break;
                    case "not":
                        AddArithmeticHeader(1);
                        hackCode.Add("D=!D");
                        AddArithmeticFooter(1);
                        break;// return TranslateToHackAssem(index + 1);
                    case "and":
                        AddArithmeticHeader();
                        hackCode.Add("D=D&M");
                        AddArithmeticFooter();
                        break;// return TranslateToHackAssem(index + 1);
                    case "or":
                        AddArithmeticHeader();
                        hackCode.Add("D=D|M");
                        AddArithmeticFooter();
                        break;// return TranslateToHackAssem(index + 1);
                    case "eq":
                        AddBooleanHeader();
                        //EQ specific
                        hackCode.Add("D;JEQ");
                        //Boolean Instructions
                        AddBooleanFooter();
                        break;// return TranslateToHackAssem(index + 1);
                    case "lt":
                        AddBooleanHeader();
                        hackCode.Add("D;JLT");
                        AddBooleanFooter();
                        break;// return TranslateToHackAssem(index + 1);
                    case "gt":
                        AddBooleanHeader();
                        hackCode.Add("D;JGT");
                        AddBooleanFooter();
                        break;// return TranslateToHackAssem(index + 1);
                    default:
                        break;
                }
                index++; 
            }
        }

        private void Pop()
        {
            hackCode.Add("D=M");
            Push();
        }

        private void PushPointer(int index)
        {
            hackCode.Add("D=A");
            hackCode.Add("@" + parsedInput[index + 2]);
            hackCode.Add("A=A+D");
        }

        private void Push()
        {
            hackCode.Add("@SP");
            hackCode.Add("A=M");
            hackCode.Add("M=D");
            hackCode.Add("@SP");
            hackCode.Add("M=M+1");
        }

        public void AddBooleanHeader()
        {
            AddArithmeticHeader();
            hackCode.Add("D=D-M");
            hackCode.Add("@TRUE_" + _arithLabelCnt);
        }
        public void AddBooleanFooter()
        {
            //Setting bool value
            hackCode.Add("@END_" + _arithLabelCnt);
            hackCode.Add("D=0;JMP");
            hackCode.Add("(TRUE_" + _arithLabelCnt + ")");
            hackCode.Add("D=-1");
            hackCode.Add("(END_" + _arithLabelCnt + ")");
            hackCode.Add("@SP");
            //Set Address to value of SP
            hackCode.Add("A=M");
            AddArithmeticFooter();
            _arithLabelCnt++;
        }
        public void AddArithmeticHeader(int parameters = 2)
        {
            if (parameters == 2)
            {
                hackCode.Add("@SP");
                hackCode.Add("M=M-1");
                hackCode.Add("A=M-1");
                hackCode.Add("D=M");
                hackCode.Add("A=A+1");
            }
            else
            {
                hackCode.Add("@SP");
                hackCode.Add("A=M-1");
                hackCode.Add("D=M");
            }
        }
        public void AddArithmeticFooter(int parameters = 2)
        {
            if(parameters == 2)
            {
                hackCode.Add("A=A-1");
                hackCode.Add("M=D");
            }
            else
            {
                hackCode.Add("M=D");
            }
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