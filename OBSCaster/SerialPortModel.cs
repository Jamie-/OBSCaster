using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OBSCaster {
    class SerialPortModel {

        // Get the COM port associated with the USB VID and PID
        public static List<string> ComPortFromIDs(String VID, String PID) {
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> ports = new List<string>();

            foreach (String s3 in rk2.GetSubKeyNames()) {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames()) {
                    if (_rx.Match(s).Success) {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames()) {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            String portName = (string) rk6.GetValue("PortName");
                            if (!String.IsNullOrWhiteSpace(portName)) {
                                ports.Add(portName);
                            }
                        }
                    }
                }
            }
            return ports;
        }
    }
}
