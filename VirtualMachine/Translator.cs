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
        int _returnAddress = 0;
        Dictionary<string, int> functionInformation;
        Stack<string> _stackTrace;

        internal void LoadFile(string fileName)
        {
            fileContents = File.ReadAllLines(fileName);
        }
        internal void MultLoadFile(string fileName)
        {
            List<string> mergedFiles = new List<string>();
            if(fileContents != null)
            {   
                mergedFiles.AddRange(fileContents);
            }
            string[] fileTemp = File.ReadAllLines(fileName);
            List<string> newFile = new List<string>();
            foreach(string s in fileTemp)
            {
                if (s.Contains("function"))
                {
                    string[] lineTmp = s.Split(' ');
                    lineTmp[1] = lineTmp[1].Replace('.', '$');
                    newFile.Add(lineTmp[0] +" "+ lineTmp[1] +" "+ lineTmp[2]);
                }
                else if (s.Contains("call"))
                {
                    string[] lineTmp = s.Split(' ');
                    lineTmp[1] = lineTmp[1].Replace('.', '$');
                    newFile.Add(lineTmp[0] + " " + lineTmp[1] + " " + lineTmp[2]);
                }
                else
                {
                    newFile.Add(s);
                }
            }
            mergedFiles.AddRange(newFile);
            fileContents = mergedFiles.ToArray();
        }
        internal void Parse()
        {
            parsedInput = Parser.Parse(fileContents);
        }
        internal void TranslateToHackAssem(bool generateBootStrap)
        {
            hackCode = new List<string>();
            if (generateBootStrap) { 
                //Load Stack Pointer
                hackCode.Add("@256");
                hackCode.Add("D=A");
                hackCode.Add("@SP");
                hackCode.Add("M=D");
                //call Sys.init
                writeCall("Sys$init", "0");
            }
            _arithLabelCnt = 0;
            functionInformation = new Dictionary<string, int>();
            _stackTrace = new Stack<string>();
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
                        _stackTrace.Push(parsedInput[index + 1].Split('$')[0]);
                        hackCode.Add("(" + parsedInput[index + 1] + ")");       
                        for(int i = 0; i < int.Parse(parsedInput[index + 2]); i++)
                        {
                            hackCode.Add("@" + 0);
                            hackCode.Add("D=A");
                            Push();
                        }                
                        break;
                    case "return":
                        _stackTrace.Pop();
                        //Store The value @LCL
                        //into a temp frame
                        hackCode.Add("@LCL");
                        hackCode.Add("D=M");
                        hackCode.Add("@14");
                        hackCode.Add("M=D");
                        hackCode.Add("@5");
                        hackCode.Add("A=D-A");
                        hackCode.Add("D=M");
                        //Grab the return address
                        //and put it in a temp variable
                        hackCode.Add("@15");
                        hackCode.Add("M=D");
                        hackCode.Add("@ARG");
                        hackCode.Add("D=M");
                        hackCode.Add("@0");
                        GenericPop();
                        //Fix the stack pointer
                        hackCode.Add("@ARG");
                        hackCode.Add("D=M+1");
                        hackCode.Add("@SP");
                        hackCode.Add("M=D");
                        //Restore the that
                        hackCode.Add("@14");
                        hackCode.Add("AM=M-1");
                        hackCode.Add("D=M");
                        hackCode.Add("@THAT");
                        hackCode.Add("M=D");
                        //Restore the this 
                        hackCode.Add("@14");
                        hackCode.Add("AM=M-1");
                        hackCode.Add("D=M");
                        hackCode.Add("@THIS");
                        hackCode.Add("M=D");
                        //Restore the Arg
                        hackCode.Add("@14");
                        hackCode.Add("AM=M-1");
                        hackCode.Add("D=M");
                        hackCode.Add("@ARG");
                        hackCode.Add("M=D");
                        //Fix the Local
                        hackCode.Add("@14");
                        hackCode.Add("AM=M-1");
                        hackCode.Add("D=M");
                        hackCode.Add("@LCL");
                        hackCode.Add("M=D");
                        //goto retAddr
                        hackCode.Add("@15");
                        hackCode.Add("A=M");
                        hackCode.Add("0;JMP");
                        break;
                    case "call":
                        writeCall(parsedInput[index + 1], parsedInput[index + 2]);
                        /*//push returnAddress
                        hackCode.Add("@" + "return_" + _returnAddress);
                        hackCode.Add("D=A");
                        Push();
                        //push Local
                        hackCode.Add("@LCL");
                        hackCode.Add("D=M");
                        Push();
                        //push Arg
                        hackCode.Add("@ARG");
                        hackCode.Add("D=M");
                        Push();
                        //push THIS
                        hackCode.Add("@THIS");
                        hackCode.Add("D=M");
                        Push();
                        //push That
                        hackCode.Add("@THAT");
                        hackCode.Add("D=M");
                        Push();
                        //ARG = SP-nArgs-5
                        hackCode.Add("@SP");
                        hackCode.Add("D=M");
                        hackCode.Add("@" + parsedInput[index + 2]);
                        hackCode.Add("D=D-A");
                        hackCode.Add("@5");
                        hackCode.Add("D=D-A");
                        Push();
                        hackCode.Add("@ARG");
                        hackCode.Add("D=A");
                        GenericPop();
                        //LCL = SP
                        hackCode.Add("@SP");
                        hackCode.Add("D=M");
                        Push();
                        hackCode.Add("@LCL");
                        hackCode.Add("D=A");
                        GenericPop();
                        //goto g
                        hackCode.Add("@" + parsedInput[index + 1]);
                        hackCode.Add("0;JMP");
                        //returnAddress:
                        hackCode.Add("(" + "return_" + _returnAddress+")");
                        _returnAddress++;*/
                        break;
                }
                index++; 
            }
        }

        private void writeCall(string funcName, string numArgs)
        {
            //push returnAddress
            hackCode.Add("@" + "return_" + _returnAddress);
            hackCode.Add("D=A");
            Push();
            //push Local
            hackCode.Add("@LCL");
            hackCode.Add("D=M");
            Push();
            //push Arg
            hackCode.Add("@ARG");
            hackCode.Add("D=M");
            Push();
            //push THIS
            hackCode.Add("@THIS");
            hackCode.Add("D=M");
            Push();
            //push That
            hackCode.Add("@THAT");
            hackCode.Add("D=M");
            Push();
            //ARG = SP-nArgs-5
            hackCode.Add("@SP");
            hackCode.Add("D=M");
            hackCode.Add("@" + numArgs);
            hackCode.Add("D=D-A");
            hackCode.Add("@5");
            hackCode.Add("D=D-A");
            Push();
            hackCode.Add("@ARG");
            hackCode.Add("D=A");
            GenericPop();
            //LCL = SP
            hackCode.Add("@SP");
            hackCode.Add("D=M");
            Push();
            hackCode.Add("@LCL");
            hackCode.Add("D=A");
            GenericPop();
            //goto g
            hackCode.Add("@" + funcName);
            hackCode.Add("0;JMP");
            //returnAddress:
            hackCode.Add("(" + "return_" + _returnAddress + ")");
            _returnAddress++;
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
                hackCode.Add("D=A");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "static")
            {
                //hackCode.Add("@" + //(16 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("@" + _stackTrace.Peek() + "." + parsedInput[index + 2]);
                hackCode.Add("D=A");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "0")
            {
                hackCode.Add("@3");
                hackCode.Add("D=A");
                GenericPop();
            }
            else if (parsedInput[index + 1] == "pointer" && parsedInput[index + 2] == "1")
            {
                hackCode.Add("@4");
                hackCode.Add("D=A");
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
                //hackCode.Add("@" + (16 + int.Parse(parsedInput[index + 2])));
                hackCode.Add("@" + _stackTrace.Peek() + "." + parsedInput[index + 2]);
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
            hackCode.Add("D=D+A");
            GenericPop();
        }
        /// <summary>
        /// Assumes you have pushed to the top of the stack the value you want.
        /// Assumes that D has the address you want.
        /// </summary>
        private void GenericPop()
        {
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