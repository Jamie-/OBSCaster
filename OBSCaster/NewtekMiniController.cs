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
            port.BaudRate = 115200;
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
            string data = sp.ReadExisting();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            var hexString = BitConverter.ToString(bytes);
            Console.WriteLine($"Data received: {hexString}");
        }
    }
}
