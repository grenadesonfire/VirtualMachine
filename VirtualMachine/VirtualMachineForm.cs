using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualMachine
{
    public partial class VirtualMachineTranslatorForm : Form
    {
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //
        // Constants
        //
        private static string PROMPT = "Prompt> ";
        private static string[] commands =
        {
            "VMtranslator"
        };

        Translator translator;

        public VirtualMachineTranslatorForm()
        {
            InitializeComponent();

            translator = new Translator();

            CommandLineTextBox.Text = PROMPT;
            EnforceCursorPosition();
            log.Info("Virtual Machine GUI has been initialized.");
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Back &&
                CommandLineTextBox.SelectionStart <= PROMPT.Length)
            {
                e.SuppressKeyPress = true;
            }
            else if(e.KeyCode == Keys.Left &&
                CommandLineTextBox.SelectionStart <= PROMPT.Length) {
                e.SuppressKeyPress = true;
                EnforceCursorPosition();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                string command = CommandLineTextBox.Text.Substring(
                    PROMPT.Length,
                    CommandLineTextBox.Text.Length-PROMPT.Length);
                e.SuppressKeyPress = true;
                //Reset the command line box
                CommandLineTextBox.Text = PROMPT;
                EnforceCursorPosition();
                //Process user command
                ProcessCommand(command);
            }
        }
        private void ProcessCommand(string command)
        {
            string[] cmdBits = command.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (cmdBits.Length < 0) {
                return;
            }
            switch (cmdBits[0])
            {
                case "vmt":
                    if(cmdBits.Length > 1)
                    {
                        if (File.Exists(cmdBits[1]))
                        {
                            translator.LoadFile(cmdBits[1]);
                            translator.Parse();
                            translator.TranslateToHackAssem(true);
                            translator.Save(cmdBits[1].Replace(".vm",".asm"));
                        }
                        else
                        {
                            HistoryTextBox.Text += "'" + cmdBits[1] + "' doesn't exist." + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    HistoryTextBox.Text += "'"+command +"' is not a supported command."+ Environment.NewLine;
                    break;
                
            }
        }
        private void CommandLineTextBox_Click(object sender, EventArgs e)
        {
            EnforceCursorPosition();
        }
        private void EnforceCursorPosition()
        {
            CommandLineTextBox.SelectionStart = PROMPT.Length;
            CommandLineTextBox.SelectionLength = 0;
        }
        private void loadVmFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Hack VM Files | *.vm";
            ofd.Multiselect = true;
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                if(ofd.FileNames.Count() == 1)
                {
                    string file = ofd.FileName;
                    translator.LoadFile(file);
                    vmFileTextBox.Lines = translator.fileContents;
                }
                else if(ofd.FileNames.Count() > 1)
                {
                    foreach (string fName in ofd.FileNames)
                        translator.MultLoadFile(fName);
                    vmFileTextBox.Lines = translator.fileContents;
                }
            }
        }

        private void Translate_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.Filter = "Hack Assembly Files | *.asm";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                string file = sfd.FileName;
                translator.Parse();
                translator.TranslateToHackAssem(!bootStrapCheckBox.Checked);
                translator.Save(file);

                //asmFileTextBox.Lines = translator.hackCode.ToArray();
                string[] asm = translator.hackCode.ToArray();
                asmFileTextBox.Text = "";
                for(int i=0;i<asm.Length;i++)
                {
                    asmFileTextBox.Text += string.Format("{0}: {1}", i, asm[i]) + Environment.NewLine;
                }
            }
        }
    }
}
