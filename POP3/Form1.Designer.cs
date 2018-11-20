namespace POP3
{
    partial class Form1
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
            this.connectButton = new System.Windows.Forms.Button();
            this.refreshMailButton = new System.Windows.Forms.Button();
            this.mailListBox = new System.Windows.Forms.ListBox();
            this.newMailLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(12, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // refreshMailButton
            // 
            this.refreshMailButton.Enabled = false;
            this.refreshMailButton.Location = new System.Drawing.Point(93, 12);
            this.refreshMailButton.Name = "refreshMailButton";
            this.refreshMailButton.Size = new System.Drawing.Size(75, 23);
            this.refreshMailButton.TabIndex = 1;
            this.refreshMailButton.Text = "Refresh";
            this.refreshMailButton.UseVisualStyleBackColor = true;
            this.refreshMailButton.Click += new System.EventHandler(this.refreshMailButton_Click);
            // 
            // mailListBox
            // 
            this.mailListBox.FormattingEnabled = true;
            this.mailListBox.Location = new System.Drawing.Point(12, 41);
            this.mailListBox.Name = "mailListBox";
            this.mailListBox.Size = new System.Drawing.Size(267, 342);
            this.mailListBox.TabIndex = 2;
            // 
            // newMailLabel
            // 
            this.newMailLabel.AutoSize = true;
            this.newMailLabel.Location = new System.Drawing.Point(214, 17);
            this.newMailLabel.Name = "newMailLabel";
            this.newMailLabel.Size = new System.Drawing.Size(65, 13);
            this.newMailLabel.TabIndex = 3;
            this.newMailLabel.Text = "No new mail";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 406);
            this.Controls.Add(this.newMailLabel);
            this.Controls.Add(this.mailListBox);
            this.Controls.Add(this.refreshMailButton);
            this.Controls.Add(this.connectButton);
            this.Name = "Form1";
            this.Text = "Pop3 - Mariusz Kozłowski";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button refreshMailButton;
        private System.Windows.Forms.ListBox mailListBox;
        private System.Windows.Forms.Label newMailLabel;
    }
}

