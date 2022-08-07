using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace OBSCaster {
    class NewtekMiniController : NewtekController {
        private SerialPort port;
        private Thread readThread;
        private bool _runReadThread;
        private char[] readBuff;
        private int readBuffPointer;

        public NewtekMiniController() {
            // Console.WriteLine("Created new NewtekMiniController instance");
        }

        public static string deviceName() {
            return "Newtek Mini Control Surface";
        }

        protected override void _connect(string serialPortName) {
            port = new SerialPort(serialPortName);
            port.BaudRate = 9600;
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            port.Open();
            readThread = new Thread(readSerialPort);
            _runReadThread = true;
            readBuff = new char[4];
            readBuffPointer = 0;
            readThread.Start();
        }

        // Internal serial read thread
        private void readSerialPort() {
            Console.WriteLine("Starting serial read thread...");
            while (_runReadThread) {
                try {
                    Int32 chunk = port.ReadChar();
                    if (readBuffPointer == 4) {
                        if (chunk == 13) {
                            // Dispatch event
                            string cmd = new string(readBuff);
                            decodeCommand(cmd);
                        } else {
                            // Got something weird - just reset the pointer and move on
                            Console.WriteLine("Received malformed data!");
                        }
                        readBuffPointer = 0;
                    } else {
                        readBuff[readBuffPointer] = (char) chunk;
                        readBuffPointer++;
                    }
                } catch (TimeoutException) { }
            }
            Console.WriteLine("Serial read thread exited!");
        }

        // Close internal read thread and serial port
        protected override void _disconnect() {
            _runReadThread = false;
            readThread.Join();
            port.Close();
        }

        public override bool supportsBacklight() {
            return true;
        }

        // Set backlight
        public override void setBacklight(int level) {
            Console.WriteLine($"Setting backlight to level: {level}");
            Debug.Assert(level >= 0 && level <= 7);
            port.Write($"070{level}\r");
        }

        private void decodeCommand(string command) {
            if (command.Length != 4) {
                Console.WriteLine($"Invalid command from RS-8: {command}");
                return;
            }
            int bank = int.Parse(command.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int values = int.Parse(command.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            //Console.WriteLine($"Bank: {bank}, Values: {values}");
            if (bank >= 16 && bank <= 23) {
                Console.WriteLine("Button");
                return;
            }
            if (bank == 24) {
                Console.WriteLine("Right knob");
                return;
            }
            if (bank == 25) {
                Console.WriteLine("Left knob");
                return;
            }
            if (bank == 128) {
                Console.WriteLine("TBar");
                return;
            }
            Console.WriteLine("UNKNOWN BANK!");
        }
    }
}
