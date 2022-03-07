using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace OBSCaster {
    public partial class Form1 : Form {
        private SerialPort sp;

        public Form1() {
            InitializeComponent();
            notifyIcon1.Icon = Properties.Resources.pizza;

        }

        private void Form1_Load(object sender, EventArgs e) {
            this.tbSettingsConsole.Items.AddRange(new string[] {
                "Newtek Mini Control Surface",
                "Newtek RS-8",
            });
            this.tbSettingsConsole.SelectedIndex = 0;
            this.tbSettingsBacklight.Items.AddRange(new string[] {
                "Off",
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
            });
            this.tbSettingsBacklight.SelectedIndex = 0;
            var ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            Console.WriteLine($"Com port: {ports[0]}");

        }

        private void ctxMenuExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void ctxMenuSettingsClick(object sender, EventArgs e) {
            this.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
                return;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.Show();
        }

        private void tbSettingsBacklight_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
