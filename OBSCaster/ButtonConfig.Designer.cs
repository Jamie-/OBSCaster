namespace OBSCaster {
	partial class ButtonConfig {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			// 
			// tableLayout
			// 
			this.tableLayout.AutoScroll = true;
			this.tableLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
			this.tableLayout.ColumnCount = 3;
			this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
			this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
			this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayout.Location = new System.Drawing.Point(12, 12);
			this.tableLayout.Name = "tableLayout";
			this.tableLayout.RowCount = 1;
			this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayout.Size = new System.Drawing.Size(776, 426);
			this.tableLayout.TabIndex = 0;
			// 
			// ButtonConfig
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tableLayout);
			this.Name = "ButtonConfig";
			this.Text = "ButtonConfig";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayout;
	}
}