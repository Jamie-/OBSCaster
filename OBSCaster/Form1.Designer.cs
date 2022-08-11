namespace OBSCaster
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
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxMenuHeader = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuDivider1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMenuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbSettingsBacklight = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bConnect = new System.Windows.Forms.Button();
            this.tbSettingsConsoleType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbHandlerPort = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.tbHandlerPassword = new System.Windows.Forms.TextBox();
            this.tbHandlerIp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbHandlerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "OBSCaster";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxMenuHeader,
            this.ctxMenuDivider1,
            this.ctxMenuSettings,
            this.ctxMenuExit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(130, 76);
            // 
            // ctxMenuHeader
            // 
            this.ctxMenuHeader.Enabled = false;
            this.ctxMenuHeader.Name = "ctxMenuHeader";
            this.ctxMenuHeader.Size = new System.Drawing.Size(129, 22);
            this.ctxMenuHeader.Text = "OBSCaster";
            // 
            // ctxMenuDivider1
            // 
            this.ctxMenuDivider1.Name = "ctxMenuDivider1";
            this.ctxMenuDivider1.Size = new System.Drawing.Size(126, 6);
            // 
            // ctxMenuSettings
            // 
            this.ctxMenuSettings.Name = "ctxMenuSettings";
            this.ctxMenuSettings.Size = new System.Drawing.Size(129, 22);
            this.ctxMenuSettings.Text = "Settings";
            this.ctxMenuSettings.Click += new System.EventHandler(this.ctxMenuSettingsClick);
            // 
            // ctxMenuExit
            // 
            this.ctxMenuExit.Name = "ctxMenuExit";
            this.ctxMenuExit.Size = new System.Drawing.Size(129, 22);
            this.ctxMenuExit.Text = "Exit";
            this.ctxMenuExit.Click += new System.EventHandler(this.ctxMenuExit_Click);
            // 
            // tbSettingsBacklight
            // 
            this.tbSettingsBacklight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tbSettingsBacklight.FormattingEnabled = true;
            this.tbSettingsBacklight.Location = new System.Drawing.Point(73, 49);
            this.tbSettingsBacklight.Name = "tbSettingsBacklight";
            this.tbSettingsBacklight.Size = new System.Drawing.Size(121, 21);
            this.tbSettingsBacklight.TabIndex = 2;
            this.tbSettingsBacklight.SelectedIndexChanged += new System.EventHandler(this.tbSettingsBacklight_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Backlight";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bConnect);
            this.groupBox1.Controls.Add(this.tbSettingsConsoleType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbSettingsBacklight);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 112);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Console Settings";
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(6, 83);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(188, 23);
            this.bConnect.TabIndex = 6;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // tbSettingsConsoleType
            // 
            this.tbSettingsConsoleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tbSettingsConsoleType.FormattingEnabled = true;
            this.tbSettingsConsoleType.Location = new System.Drawing.Point(73, 22);
            this.tbSettingsConsoleType.Name = "tbSettingsConsoleType";
            this.tbSettingsConsoleType.Size = new System.Drawing.Size(121, 21);
            this.tbSettingsConsoleType.TabIndex = 1;
            this.tbSettingsConsoleType.SelectedIndexChanged += new System.EventHandler(this.tbSettingsConsoleType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Console";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbHandlerPort);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbHandlerPassword);
            this.groupBox2.Controls.Add(this.tbHandlerIp);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(218, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 112);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OBS Settings";
            // 
            // tbHandlerPort
            // 
            this.tbHandlerPort.Location = new System.Drawing.Point(62, 52);
            this.tbHandlerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.tbHandlerPort.Name = "tbHandlerPort";
            this.tbHandlerPort.Size = new System.Drawing.Size(120, 20);
            this.tbHandlerPort.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Secret";
            // 
            // tbHandlerPassword
            // 
            this.tbHandlerPassword.Location = new System.Drawing.Point(62, 77);
            this.tbHandlerPassword.Name = "tbHandlerPassword";
            this.tbHandlerPassword.Size = new System.Drawing.Size(132, 20);
            this.tbHandlerPassword.TabIndex = 5;
            // 
            // tbHandlerIp
            // 
            this.tbHandlerIp.Location = new System.Drawing.Point(62, 25);
            this.tbHandlerIp.Name = "tbHandlerIp";
            this.tbHandlerIp.Size = new System.Drawing.Size(132, 20);
            this.tbHandlerIp.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "IP";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 133);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "OBSCaster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbHandlerPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuSettings;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuExit;
        private System.Windows.Forms.ToolStripSeparator ctxMenuDivider1;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuHeader;
        private System.Windows.Forms.ComboBox tbSettingsBacklight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox tbSettingsConsoleType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbHandlerPassword;
        private System.Windows.Forms.TextBox tbHandlerIp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown tbHandlerPort;
        private System.Windows.Forms.Button bConnect;
    }
}

