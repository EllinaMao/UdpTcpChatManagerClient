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
            components = new System.ComponentModel.Container();
            send_btn = new Button();
            connect_btn = new Button();
            textBoxMessage = new TextBox();
            textBoxUsername = new TextBox();
            button1 = new Button();
            userListBox = new ListBox();
            imageList1 = new ImageList(components);
            systemLogListBox = new ListBox();
            tabPage1 = new TabPage();
            mainChatListBox = new ListBox();
            tabControl1 = new TabControl();
            textBoxIp = new TextBox();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // send_btn
            // 
            send_btn.Location = new Point(405, 415);
            send_btn.Name = "send_btn";
            send_btn.Size = new Size(63, 23);
            send_btn.TabIndex = 3;
            send_btn.Text = "Send";
            send_btn.UseVisualStyleBackColor = true;
            send_btn.Click += send_btn_Click;
            // 
            // connect_btn
            // 
            connect_btn.Location = new Point(451, 12);
            connect_btn.Name = "connect_btn";
            connect_btn.Size = new Size(90, 23);
            connect_btn.TabIndex = 4;
            connect_btn.Text = "Connect";
            connect_btn.UseVisualStyleBackColor = true;
            connect_btn.Click += connect_btn_Click;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new Point(31, 415);
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(368, 23);
            textBoxMessage.TabIndex = 5;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(191, 12);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.PlaceholderText = "Your name";
            textBoxUsername.Size = new Size(254, 23);
            textBoxUsername.TabIndex = 6;
            // 
            // button1
            // 
            button1.Location = new Point(474, 415);
            button1.Name = "button1";
            button1.Size = new Size(67, 23);
            button1.TabIndex = 8;
            button1.Text = "SendPM";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // userListBox
            // 
            userListBox.FormattingEnabled = true;
            userListBox.ItemHeight = 15;
            userListBox.Location = new Point(554, 12);
            userListBox.Name = "userListBox";
            userListBox.Size = new Size(234, 319);
            userListBox.TabIndex = 9;
            userListBox.DoubleClick += userListBox_DoubleClick;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // systemLogListBox
            // 
            systemLogListBox.FormattingEnabled = true;
            systemLogListBox.ItemHeight = 15;
            systemLogListBox.Location = new Point(554, 340);
            systemLogListBox.Name = "systemLogListBox";
            systemLogListBox.Size = new Size(234, 94);
            systemLogListBox.TabIndex = 11;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(mainChatListBox);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(516, 340);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "All chat";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // mainChatListBox
            // 
            mainChatListBox.Dock = DockStyle.Fill;
            mainChatListBox.FormattingEnabled = true;
            mainChatListBox.ItemHeight = 15;
            mainChatListBox.Location = new Point(3, 3);
            mainChatListBox.Name = "mainChatListBox";
            mainChatListBox.Size = new Size(510, 334);
            mainChatListBox.TabIndex = 2;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(24, 41);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(524, 368);
            tabControl1.TabIndex = 10;
            // 
            // textBoxIp
            // 
            textBoxIp.Location = new Point(28, 13);
            textBoxIp.Name = "textBoxIp";
            textBoxIp.PlaceholderText = "Ip";
            textBoxIp.Size = new Size(157, 23);
            textBoxIp.TabIndex = 12;
            textBoxIp.Text = "127.0.0.1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBoxIp);
            Controls.Add(systemLogListBox);
            Controls.Add(tabControl1);
            Controls.Add(userListBox);
            Controls.Add(button1);
            Controls.Add(textBoxUsername);
            Controls.Add(textBoxMessage);
            Controls.Add(connect_btn);
            Controls.Add(send_btn);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            tabPage1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button send_btn;
        private Button connect_btn;
        private TextBox textBoxMessage;
        private TextBox textBoxUsername;
        private Button button1;
        private ListBox userListBox;
        private ImageList imageList1;
        private ListBox systemLogListBox;
        private TabPage tabPage1;
        private ListBox mainChatListBox;
        private TabControl tabControl1;
        private TextBox textBoxIp;
    }
}
