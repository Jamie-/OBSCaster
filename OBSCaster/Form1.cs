using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace OBSCaster {
    public partial class Form1 : Form {
        private NewtekController controller;

        public Form1() {
            InitializeComponent();
            notifyIcon1.Icon = Properties.Resources.pizza;

            // Register for USB device connects and disconnects
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange) {
                switch ((int)m.WParam) {
                    case UsbNotification.DbtDeviceremovecomplete:
						Console.WriteLine($"USB Device removed!!");
                        break;
                    case UsbNotification.DbtDevicearrival:
                        Console.WriteLine($"USB Device connected!!");
                        break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.tbSettingsConsoleType.Items.AddRange(new string[] {
                "Newtek Mini Control Surface",
                "Newtek RS-8",
            });
            this.tbSettingsConsoleType.SelectedIndex = 0;
            Dictionary<string, int> backlightDict = new Dictionary<string, int>() {
                {"Off", 0},
                {"1", 1},
                {"2", 2},
                {"3", 3},
                {"4", 4},
                {"5", 5},
                {"6", 6},
                {"7", 7},
            };
            this.tbSettingsBacklight.DataSource = new BindingSource(backlightDict, null);
            this.tbSettingsBacklight.ValueMember = "Value";
            this.tbSettingsBacklight.DisplayMember = "Key";
            this.tbSettingsBacklight.SelectedIndex = 0;
            var ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            Console.WriteLine($"Com port: {ports[0]}");
            
            //Console.WriteLine($"Com port: {ports[0]}");
            //this.sp = new SerialPort(ports[0], 9600);
            //this.sp.NewLine = "\r";
            //this.sp.Open();
            //this.sp.WriteLine("00FF");
            //this.sp.Close();
            //Console.WriteLine("hi");
        }

        // Handle exit
        private void ctxMenuExit_Click(object sender, EventArgs e) {
            if (this.controller != null && this.controller.IsConnected()) {
                Console.WriteLine("Disconnecting before shutdown...");
                this.controller.disconnect();
            }
            Application.Exit();
        }

        // Show form on context menu settings
        private void ctxMenuSettingsClick(object sender, EventArgs e) {
            this.Show();
        }

        // Handle settings form close
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            // Keep notify icon in tray until we exit via the notify context menu
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
                return;
            }
        }

        // Handle settings form open
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.Show();
        }

        // Set backlight on change
        private void tbSettingsBacklight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.controller.IsConnected() && this.controller.supportsBacklight()) {
                this.controller.setBacklight((int)tbSettingsBacklight.SelectedValue);
            }
        }

        // Change device type
        private void tbSettingsConsoleType_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox tbSettingsConsoleType = (ComboBox) sender;
            if (this.controller != null && this.controller.IsConnected()) {
                // TODO: if we pick the same controller type we shouldn't disconnect!
                this.controller.disconnect();
            }
            // Set new controller type
            if (tbSettingsConsoleType.SelectedItem.ToString() == NewtekMiniController.deviceName()) {
                this.controller = new NewtekMiniController();
            } else if (tbSettingsConsoleType.SelectedItem.ToString() == NewTekRS_8.deviceName()) {
                this.controller = new NewTekRS_8();
            }
            // Enable/disable backlight settings depending on if supported or not
            if (this.controller.supportsBacklight()) {
                this.tbSettingsBacklight.Enabled = true;
            } else {
                this.tbSettingsBacklight.Enabled = false;
            }
        }

        // Connect and disconnect serial port
        private void bConnect_Click(object sender, EventArgs e) {
            Button bConnect = (Button) sender;
            if (this.controller == null) {
                MessageBox.Show("Controller type must be set to connect!");
                return;
            }
            if (this.controller.IsConnected()) {
                bConnect.Enabled = false;
                this.controller.disconnect();
                bConnect.Text = "Connect";
                bConnect.Enabled = true;
            } else {
                bConnect.Enabled = false;
                this.controller.connect();
                // Set backlight value after connect
                if (this.controller.supportsBacklight()) {
                    this.controller.setBacklight((int) tbSettingsBacklight.SelectedValue);
                }
                bConnect.Text = "Disconnect";
                bConnect.Enabled = true;
            }
        }
    }
}
