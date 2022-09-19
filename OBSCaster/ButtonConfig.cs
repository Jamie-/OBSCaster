using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBSCaster {
	public partial class ButtonConfig : Form {
		string[] buttons;
		public ButtonConfig(string [] optionButtons) {
			InitializeComponent();
			this.buttons = optionButtons;

			for (int i = 0; i < optionButtons.Length; i++) {
                AddRowToPanel(tableLayout, new Control[] { new Label() { Text = optionButtons[i] }, new Button() { Text = "wow" }, new TextBox() });
			}
		}

        private void AddRowToPanel(TableLayoutPanel panel, Control[] rowElements) {
            if (panel.ColumnCount != rowElements.Length)
                throw new Exception("Elements number doesn't match!");
            //get a reference to the previous existent row
            RowStyle temp = panel.RowStyles[panel.RowCount - 1];
            //increase panel rows count by one
            panel.RowCount++;
            //add a new RowStyle as a copy of the previous one
            panel.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));
            //add the control
            for (int i = 0; i < rowElements.Length; i++) {
                rowElements[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                panel.Controls.Add(rowElements[i], i, panel.RowCount - 1);
            }
        }
    }
}
