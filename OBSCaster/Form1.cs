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
            var ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            Console.WriteLine($"Com port: {ports[0]}");
            this.sp = new SerialPort(ports[0], 9600);
            this.sp.NewLine = "\r";
            this.sp.Open();
            this.sp.WriteLine("00FF");
            this.sp.Close();
            Console.WriteLine("hi");

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
    }
}
