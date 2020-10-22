namespace NeatDiggersPrototype
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.createRoomButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.serverTimer = new System.Windows.Forms.Timer(this.components);
            this.codeLabel = new System.Windows.Forms.Label();
            this.nickNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // createRoomButton
            // 
            this.createRoomButton.Location = new System.Drawing.Point(109, 170);
            this.createRoomButton.Name = "createRoomButton";
            this.createRoomButton.Size = new System.Drawing.Size(105, 29);
            this.createRoomButton.TabIndex = 0;
            this.createRoomButton.Text = "Create room";
            this.createRoomButton.Click += new System.EventHandler(this.createRoomButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(109, 250);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(105, 29);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // serverTimer
            // 
            this.serverTimer.Tick += new System.EventHandler(this.serverTimer_Tick);
            // 
            // codeLabel
            // 
            this.codeLabel.AutoSize = true;
            this.codeLabel.Location = new System.Drawing.Point(143, 218);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(35, 15);
            this.codeLabel.TabIndex = 1;
            this.codeLabel.Text = "Code";
            // 
            // nickNameTextBox
            // 
            this.nickNameTextBox.Location = new System.Drawing.Point(109, 109);
            this.nickNameTextBox.Name = "nickNameTextBox";
            this.nickNameTextBox.Size = new System.Drawing.Size(100, 23);
            this.nickNameTextBox.TabIndex = 2;
            this.nickNameTextBox.Text = "Alex228";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 429);
            this.Controls.Add(this.nickNameTextBox);
            this.Controls.Add(this.codeLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.createRoomButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Prototype";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button createRoomButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Timer serverTimer;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.TextBox nickNameTextBox;
    }
}

