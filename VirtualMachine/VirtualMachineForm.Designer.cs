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
            this.CommandLineTextBox = new System.Windows.Forms.TextBox();
            this.HistoryTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CommandLineTextBox
            // 
            this.CommandLineTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CommandLineTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.CommandLineTextBox.BackColor = System.Drawing.Color.Black;
            this.CommandLineTextBox.ForeColor = System.Drawing.Color.Lime;
            this.CommandLineTextBox.Location = new System.Drawing.Point(13, 386);
            this.CommandLineTextBox.Name = "CommandLineTextBox";
            this.CommandLineTextBox.Size = new System.Drawing.Size(542, 20);
            this.CommandLineTextBox.TabIndex = 0;
            this.CommandLineTextBox.Click += new System.EventHandler(this.CommandLineTextBox_Click);
            this.CommandLineTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // HistoryTextBox
            // 
            this.HistoryTextBox.BackColor = System.Drawing.Color.Black;
            this.HistoryTextBox.ForeColor = System.Drawing.Color.Lime;
            this.HistoryTextBox.Location = new System.Drawing.Point(12, 12);
            this.HistoryTextBox.Multiline = true;
            this.HistoryTextBox.Name = "HistoryTextBox";
            this.HistoryTextBox.Size = new System.Drawing.Size(543, 368);
            this.HistoryTextBox.TabIndex = 1;
            // 
            // VirtualMachineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 418);
            this.Controls.Add(this.HistoryTextBox);
            this.Controls.Add(this.CommandLineTextBox);
            this.Name = "VirtualMachineForm";
            this.Text = "Virtual Machine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CommandLineTextBox;
        private System.Windows.Forms.TextBox HistoryTextBox;
    }
}

