﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace OBSCaster {
    public partial class Form1 : Form {
        private NewtekController controller;
        private OutputHandler handler;
        private bool connected = false;

        public Form1() {
            InitializeComponent();
            notifyIcon1.Icon = Properties.Resources.pizza;

            // Register for USB device connects and disconnects
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);

            handler = new OBSHandler();

            // Show settings window on startup
            this.Show();
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

            // Load settings
            tbSettingsConsoleType.Text = Properties.Settings.Default.ui_console_type;
            tbSettingsBacklight.SelectedValue = Properties.Settings.Default.ui_console_backlight;
            tbHandlerIp.Text = Properties.Settings.Default.ui_handler_ip;
            tbHandlerPort.Value = Properties.Settings.Default.ui_handler_port;
            tbHandlerPassword.Text = Properties.Settings.Default.ui_handler_secret;

            var ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            foreach (var port in ports) {
                Console.WriteLine($"Found port: {port}");
            }
        }

        // Handle exit
        private void ctxMenuExit_Click(object sender, EventArgs e) {
            if (connected) {
                Console.WriteLine("Disconnecting before shutdown...");
                controller.disconnect();
                handler.disconnect();
                connected = false;
            }
            notifyIcon1.Dispose();
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

                // Save settings
                Properties.Settings.Default.ui_console_type = tbSettingsConsoleType.Text;
                Properties.Settings.Default.ui_console_backlight = (int) tbSettingsBacklight.SelectedValue;
                Properties.Settings.Default.ui_handler_ip = tbHandlerIp.Text;
                Properties.Settings.Default.ui_handler_port = (int) tbHandlerPort.Value;
                Properties.Settings.Default.ui_handler_secret = tbHandlerPassword.Text;
                Properties.Settings.Default.Save();
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
            if (controller != null) Debug.Assert(!controller.IsConnected());
            // Set new controller type
            if (tbSettingsConsoleType.SelectedItem.ToString() == NewtekMiniController.deviceName()) {
                this.controller = new NewtekMiniController(handler);
                handler.controller = controller;
            } else if (tbSettingsConsoleType.SelectedItem.ToString() == NewTekRS_8.deviceName()) {
                this.controller = new NewTekRS_8(handler);
                handler.controller = controller;
            }
            // Enable/disable backlight settings depending on if supported or not
            if (this.controller.supportsBacklight()) {
                this.tbSettingsBacklight.Enabled = true;
            } else {
                this.tbSettingsBacklight.Enabled = false;
            }
        }

        // Connect and disconnect serial port
        private void handleConnectButtons() {
            if (this.controller == null) {
                MessageBox.Show("Controller type must be set to connect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (connected) {
                bConnect.Enabled = false;
                ctxMenuConnect.Enabled = false;
                this.controller.disconnect();
                handler.disconnect();
                connected = false;
                bConnect.Text = "Connect";
                ctxMenuConnect.Text = "Connect";
                bConnect.Enabled = true;
                ctxMenuConnect.Enabled = true;
                tbSettingsConsoleType.Enabled = true;
            } else {
                bConnect.Enabled = false;
                ctxMenuConnect.Enabled = false;
                tbSettingsConsoleType.Enabled = false;
                try {
                    this.controller.connect();
                    if (!handler.connect(tbHandlerIp.Text, (int)tbHandlerPort.Value, tbHandlerPassword.Text)) {
                        // Bail out if handler fails to connect
                        this.controller.disconnect();
                        tbSettingsConsoleType.Enabled = true;
                        bConnect.Enabled = true;
                        ctxMenuConnect.Enabled = true;
                        return;
                    }
                    // Set backlight value after connect
                    if (this.controller.supportsBacklight()) {
                        this.controller.setBacklight((int)tbSettingsBacklight.SelectedValue);
                    }
                    bConnect.Text = "Disconnect";
                    ctxMenuConnect.Text = "Disconnect";
                    connected = true;
                } catch (Exception ex) when (ex is System.IO.IOException || ex is NoSerialPortsFound) {
                    MessageBox.Show(ex.Message.ToString(), "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tbSettingsConsoleType.Enabled = true;
                }
                bConnect.Enabled = true;
                ctxMenuConnect.Enabled = true;
            }
        }

        private void bConnect_Click(object sender, EventArgs e) {
            handleConnectButtons();
        }

        private void ctxMenuConnect_Click(object sender, EventArgs e) {
            handleConnectButtons();
        }
    }
}
