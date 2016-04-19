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
        public string[] fileContents{get; private set;}
        List<string> parsedInput;
        public List<string> hackCode { get; private set; }
        int _arithLabelCnt = 0;
        Dictionary<string, int> functionInformation;

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
            functionInformation = new Dictionary<string, int>();
            TranslateToHackAssem(0);
        }
        private void TranslateToHackAssem(int index)
        {
            while (index < parsedInput.Count)
            {
                switch (parsedInput[index])
                {
                    case "push":
                        ParsePush(index);
                        index += 2;
                        break;
                    case "pop":
                        ParsePop(index);
                        index += 2;
                        break;
                    case "sub":
                    case "and":
                    case "or":
                    case "add":
                        ParseArithmetic(parsedInput[index]);
                        break;
                    case "not":
                    case "neg":
                        ParseArithmetic(parsedInput[index], 1);
                        break;
                    case "eq":
                    case "lt":
                    case "gt":
                        ParseBoolean(parsedInput[index]);
                        break;
                    case "label":
                        hackCode.Add("(" + parsedInput[index + 1] + ")");
                        break;
                    case "if-goto":
                        hackCode.Add("@SP");
                        hackCode.Add("AM=M-1");
                        hackCode.Add("D=M");
                        hackCode.Add("@" + parsedInput[index + 1]);
                        hackCode.Add("D;JNE");
                        break;
                    case "goto":
                        hackCode.Add("@" + parsedInput[index + 1]);
                        hackCode.Add("0;JMP");
                        break;
                    case "function":
                        functionInformation.Add(
                            parsedInput[index + 1], 
                            int.Parse(parsedInput[index + 2]));
                        hackCode.Add("(" + parsedInput[index + 1] + ")");       
                        for(int i = 0; i < int.Parse(parsedInput[index + 2]); i++)
                        {
                            hackCode.Add("@" + 0);
                            hackCode.Add("D=A");
                            Push();
                        }                
                        break;
                    case "return":
                        hackCode.Add("@LCL");
                        hackCode.Add("D=M");
                        hackCode.Add("@14		");
                        hackCode.Add("M=D");
                        hackCode.Add("@5");
                        hackCode.Add("A=D-A");
                        hackCode.Add("D=M");
                        hackCode.Add("@15		");
                        hackCode.Add("M=D");
                        hackCode.Add("@ARG");
                        hackCode.Add("A=M");
                        hackCode.Add("D=A");
                        hackCode.Add("@0");
                        GenericPop();
                        hackCode.Add("@14");
                        hackCode.Add("DM=M-1");
                        hackCode.Add("@THAT");
                        hackCode.Add("M=D");
                        hackCode.Add("@14");
                        hackCode.Add("DM=M-1");
                        hackCode.Add("@THIS");
                        hackCode.Add("M=D");
                        hackCode.Add("@14");
                        hackCode.Add("DM=M-1");
                        hackCode.Add("@ARG");
                        hackCode.Add("M=D");
                        hackCode.Add("@14");
                        hackCode.Add("DM=M-1");
                        hackCode.Add("@LCL");
                        hackCode.Add("M=D");
                        hackCode.Add("@15");
                        hackCode.Add("A=M");
                        hackCode.Add("0;JMP");
                        /*hackCode.Add("@SP");
                        hackCode.Add("D=M");
                        hackCode.Add("M=M-1");
                        hackCode.Add("A=D");
                        hackCode.Add("0;JMP");*/
                        break;
                }
                index++; 
            }
        }

        private void ParseBoolean(string v)
        {
            AddBooleanHeader();
            switch (v)
            {
                case "eq":
                    hackCode.Add("D;JEQ");
                    break;
                case "lt":
                    hackCode.Add("D;JLT");
                    break;
                case "gt":
                    hackCode.Add("D;JGT");
                    break;
            }
            AddBooleanFooter();
        }

        private void ParseArithmetic(string cmd, int arg = 2)
        {
            AddArithmeticHeader(arg);
            switch (cmd)
            {
                case "add":
                    hackCode.Add("D=D+M");
                    break;
                case "sub":
                    hackCode.Add("D=D-M");
                    break;
                case "neg":
                    hackCode.Add("D=-D");
                    break;
                case "not":
                    hackCode.Add("D=!D");
                    break;
                case "and":
                    hackCode.Add("D=D&M");
                    break;
                case "or":
                    hackCode.Add("D=D|M");
                    break;
            }
            AddArithmeticFooter(arg);
        }

        private void ParsePop(int index)
        {
            if (parsedInput[index + 1] == "local")
            {
                hackCode.Add("@LCL");
                Pop(index);
            }
            else if (parsedInput[index + 1] == "argument")
            {
                hackCode.Add("@ARG");
                Pop(index);
            }
            else if (parsedInput[index + 1] == "this")
            {
                hackCode.Add("@THIS");
                Pop(index);
            }
            else if (parsedInput[index + 1] == "that")
            {
                hackCode.Add("@THAT");
                Pop(index);
            }
            else if (parsedInput[index + 1] == "temp")
            {
                hackCode.Add("@" + (5 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("D=0");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "static")
            {
                hackCode.Add("@" + (16 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("D=0");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "0")
            {
                hackCode.Add("@3");
                hackCode.Add("D=0");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "1")
            {
                hackCode.Add("@4");
                hackCode.Add("D=0");
                GenericPop();
            }
        }
        private void ParsePush(int index)
        {
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
            else if (parsedInput[index + 1] == "this")
            {
                hackCode.Add("@THIS");
                PushPointer(index);
            }
            else if (parsedInput[index + 1] == "that")
            {
                hackCode.Add("@THAT");
                PushPointer(index);
            }
            else if (parsedInput[index + 1] == "temp")
            {
                hackCode.Add("@" + (5 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("D=M");
                Push();
            }
            else if (parsedInput[index + 1] == "static")
            {
                hackCode.Add("@" + (16 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("D=M");
                Push();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "0")
            {
                hackCode.Add("@THIS");
                hackCode.Add("D=M");
                Push();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "1")
            {
                hackCode.Add("@THAT");
                hackCode.Add("D=M");
                Push();
            }
        }
        private void Pop(int index)
        {
            //Assuming A has the pointer we're concerned with
            //Calculate the final address
            hackCode.Add("A=M");
            hackCode.Add("D=A");
            hackCode.Add("@" + parsedInput[index + 2]);
            GenericPop();
        }
        private void GenericPop()
        {
            hackCode.Add("D=D+A");
            //Store at top of the stack
            hackCode.Add("@SP");
            hackCode.Add("A=M");
            hackCode.Add("M=D");
            //Grab the value we wanted to pop
            hackCode.Add("@SP");
            hackCode.Add("A=M-1");
            hackCode.Add("D=M");
            //Now put that value back into memoery
            hackCode.Add("@SP");
            hackCode.Add("A=M");
            hackCode.Add("A=M");
            hackCode.Add("M=D");
            //Decrement stack pointer
            hackCode.Add("@SP");
            hackCode.Add("M=M-1");
        }
        private void PushPointer(int index)
        {
            //Resolve pointer to get address of value we want
            hackCode.Add("D=M");
            hackCode.Add("@" + parsedInput[index + 2]);
            hackCode.Add("A=D+A");
            //Grab said value
            hackCode.Add("D=M");
            //Assign to top of stack
            hackCode.Add("@SP");
            hackCode.Add("A=M");
            hackCode.Add("M=D");
            //Increment Stack Pointer
            hackCode.Add("@SP");
            hackCode.Add("M=M+1");
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