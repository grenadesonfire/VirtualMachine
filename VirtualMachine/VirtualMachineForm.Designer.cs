namespace VirtualMachine
{
    partial class VirtualMachineTranslatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HistoryTextBox = new System.Windows.Forms.TextBox();
            this.CommandLineTextBox = new System.Windows.Forms.TextBox();
            this.vmFileTextBox = new System.Windows.Forms.TextBox();
            this.asmFileTextBox = new System.Windows.Forms.TextBox();
            this.loadVmFile = new System.Windows.Forms.Button();
            this.TranslateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HistoryTextBox
            // 
            this.HistoryTextBox.BackColor = System.Drawing.Color.Black;
            this.HistoryTextBox.ForeColor = System.Drawing.Color.Lime;
            this.HistoryTextBox.Location = new System.Drawing.Point(714, 12);
            this.HistoryTextBox.Multiline = true;
            this.HistoryTextBox.Name = "HistoryTextBox";
            this.HistoryTextBox.Size = new System.Drawing.Size(543, 368);
            this.HistoryTextBox.TabIndex = 2;
            // 
            // CommandLineTextBox
            // 
            this.CommandLineTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CommandLineTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.CommandLineTextBox.BackColor = System.Drawing.Color.Black;
            this.CommandLineTextBox.ForeColor = System.Drawing.Color.Lime;
            this.CommandLineTextBox.Location = new System.Drawing.Point(715, 386);
            this.CommandLineTextBox.Name = "CommandLineTextBox";
            this.CommandLineTextBox.Size = new System.Drawing.Size(542, 20);
            this.CommandLineTextBox.TabIndex = 0;
            this.CommandLineTextBox.Click += new System.EventHandler(this.CommandLineTextBox_Click);
            this.CommandLineTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // vmFileTextBox
            // 
            this.vmFileTextBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.vmFileTextBox.ForeColor = System.Drawing.Color.Gold;
            this.vmFileTextBox.Location = new System.Drawing.Point(12, 12);
            this.vmFileTextBox.Multiline = true;
            this.vmFileTextBox.Name = "vmFileTextBox";
            this.vmFileTextBox.ReadOnly = true;
            this.vmFileTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.vmFileTextBox.Size = new System.Drawing.Size(207, 394);
            this.vmFileTextBox.TabIndex = 3;
            // 
            // asmFileTextBox
            // 
            this.asmFileTextBox.BackColor = System.Drawing.SystemColors.WindowText;
            this.asmFileTextBox.ForeColor = System.Drawing.Color.Lime;
            this.asmFileTextBox.Location = new System.Drawing.Point(225, 12);
            this.asmFileTextBox.Multiline = true;
            this.asmFileTextBox.Name = "asmFileTextBox";
            this.asmFileTextBox.ReadOnly = true;
            this.asmFileTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.asmFileTextBox.Size = new System.Drawing.Size(207, 394);
            this.asmFileTextBox.TabIndex = 4;
            // 
            // loadVmFile
            // 
            this.loadVmFile.Location = new System.Drawing.Point(12, 412);
            this.loadVmFile.Name = "loadVmFile";
            this.loadVmFile.Size = new System.Drawing.Size(75, 23);
            this.loadVmFile.TabIndex = 5;
            this.loadVmFile.Text = "Load (.vm)";
            this.loadVmFile.UseVisualStyleBackColor = true;
            this.loadVmFile.Click += new System.EventHandler(this.loadVmFile_Click);
            // 
            // TranslateButton
            // 
            this.TranslateButton.Location = new System.Drawing.Point(225, 412);
            this.TranslateButton.Name = "TranslateButton";
            this.TranslateButton.Size = new System.Drawing.Size(93, 23);
            this.TranslateButton.TabIndex = 6;
            this.TranslateButton.Text = "Translate (.asm)";
            this.TranslateButton.UseVisualStyleBackColor = true;
            this.TranslateButton.Click += new System.EventHandler(this.Translate_Click);
            // 
            // VirtualMachineTranslatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 448);
            this.Controls.Add(this.TranslateButton);
            this.Controls.Add(this.loadVmFile);
            this.Controls.Add(this.asmFileTextBox);
            this.Controls.Add(this.vmFileTextBox);
            this.Controls.Add(this.HistoryTextBox);
            this.Controls.Add(this.CommandLineTextBox);
            this.Name = "VirtualMachineTranslatorForm";
            this.Text = "Virtual Machine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HistoryTextBox;
        private System.Windows.Forms.TextBox CommandLineTextBox;
        private System.Windows.Forms.TextBox vmFileTextBox;
        private System.Windows.Forms.TextBox asmFileTextBox;
        private System.Windows.Forms.Button loadVmFile;
        private System.Windows.Forms.Button TranslateButton;
    }
}

