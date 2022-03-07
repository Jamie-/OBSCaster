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
            this.contextMenuStrip1.SuspendLayout();
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Form1";
            this.Text = "OBSCaster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuSettings;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuExit;
        private System.Windows.Forms.ToolStripSeparator ctxMenuDivider1;
        private System.Windows.Forms.ToolStripMenuItem ctxMenuHeader;
    }
}

