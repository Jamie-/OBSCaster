using System;
using System.IO.Ports;

namespace OBSCaster {
    class NewtekMiniController : NewtekController {
        private SerialPort port;

        public NewtekMiniController() {
            Console.WriteLine("Created new NewtekMiniController instance");
        }

        public static string deviceName() {
            return "Newtek Mini Control Surface";
        }

        protected override void _connect(string serialPortName) {
            port = new SerialPort(serialPortName);
            port.BaudRate = 9600;
            port.NewLine = "\r";
            port.DataReceived += new SerialDataReceivedEventHandler(dataReceivedHandler);
            port.Open();
        }

        protected override void _disconnect() {
            port.Close();
        }

        public override bool supportsBacklight() {
            return true;
        }

        // Set backlight
        public override void setBacklight(int level) {

        }

        private void dataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            // TODO: need to check when data recv handler gets called - we may need to call multiple ReadLine()
            // while bytestoread > 0 do a ReadLine
            // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            // var hexString = BitConverter.ToString(bytes);
            Console.WriteLine($"Data received: {data}");
            this.decodeCommand(data);
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
