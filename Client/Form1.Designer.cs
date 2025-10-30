namespace Client
{
    partial class Form1
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
            textBoxIp = new TextBox();
            listBox1 = new ListBox();
            send_btn = new Button();
            connect_btn = new Button();
            textBoxMessage = new TextBox();
            textBoxUsername = new TextBox();
            SuspendLayout();
            // 
            // textBoxIp
            // 
            textBoxIp.Location = new Point(24, 12);
            textBoxIp.Name = "textBoxIp";
            textBoxIp.PlaceholderText = "ip";
            textBoxIp.ReadOnly = true;
            textBoxIp.Size = new Size(170, 23);
            textBoxIp.TabIndex = 0;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(24, 56);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(478, 349);
            listBox1.TabIndex = 2;
            // 
            // send_btn
            // 
            send_btn.Location = new Point(400, 415);
            send_btn.Name = "send_btn";
            send_btn.Size = new Size(102, 23);
            send_btn.TabIndex = 3;
            send_btn.Text = "Send";
            send_btn.UseVisualStyleBackColor = true;
            send_btn.Click += send_btn_Click;
            // 
            // connect_btn
            // 
            connect_btn.Location = new Point(412, 12);
            connect_btn.Name = "connect_btn";
            connect_btn.Size = new Size(90, 23);
            connect_btn.TabIndex = 4;
            connect_btn.Text = "Connect";
            connect_btn.UseVisualStyleBackColor = true;
            connect_btn.Click += connect_btn_Click;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(24, 415);
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(370, 23);
            textBoxMessage.TabIndex = 5;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(200, 12);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(206, 23);
            textBoxUsername.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBoxUsername);
            Controls.Add(textBoxMessage);
            Controls.Add(connect_btn);
            Controls.Add(send_btn);
            Controls.Add(listBox1);
            Controls.Add(textBoxIp);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxIp;
        private ListBox listBox1;
        private Button send_btn;
        private Button connect_btn;
        private TextBox textBoxMessage;
        private TextBox textBoxUsername;
    }
}
