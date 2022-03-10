﻿using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace OBSCaster {
    public partial class Form1 : Form {
        private NewtekController controller;
        private SerialPort sp;
        private NewTekRS_8 device;
        private int temp=1;

        public Form1() {
            InitializeComponent();
            notifyIcon1.Icon = Properties.Resources.pizza;
            var ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            if (ports.Count>0) {
                device = new NewTekRS_8(ports[0]);
                device.vegasStart();
            }

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

        private void tbSettingsBacklight_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        // Change device type
        private void tbSettingsConsole_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox tbSettingsConsole = (ComboBox) sender;
            if (this.controller != null && this.controller.IsConnected()) {
                this.controller.disconnect();
            }
            if (tbSettingsConsole.SelectedItem.ToString() == NewtekMiniController.deviceName()) {
                this.controller = new NewtekMiniController();
                if (this.controller.supportsBacklight()) {

                }
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
                Console.WriteLine("Disconnecting...");
                this.controller.disconnect();
                bConnect.Text = "Connect";
                bConnect.Enabled = true;
            } else {
                bConnect.Enabled = false;
                Console.WriteLine("Connecting...");
                this.controller.connect();
                if (this.controller.supportsBacklight()) {
                    // TODO: set the backlight here to the value from tbSettingsBacklight
                    this.controller.setBacklight(7);
                }
                bConnect.Text = "Disconnect";
                bConnect.Enabled = true;
            }
        }
    }
}
