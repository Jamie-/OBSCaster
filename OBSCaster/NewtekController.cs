using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSCaster {

    public class NoSerialPortsFound : Exception {

    }

    abstract class NewtekController {
        protected bool connected = false;

        // Connect to serial port
        public void connect() {
            Console.WriteLine("Connecting to controller...");
            List<string> ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            if (ports.Count > 0) {
                if (ports.Count > 1) {
                    Console.WriteLine("WARNING: More than one serial port for IDs found!");
                }
                this._connect(ports[0]);
                this.connected = true;
                Console.WriteLine("Controller connected!");
            } else {
                throw new NoSerialPortsFound();
            }
        }

        // Internal workings of connection
        protected abstract void _connect(string serialPortName);

        // Disconnect from serial port
        public void disconnect() {
            Console.WriteLine("Disconnecting from controller...");

            this._disconnect();
            this.connected = false;
            Console.WriteLine("Controller disconnected!");
        }

        // Internal workings of disconnection
        protected abstract void _disconnect();

        public bool IsConnected() {
            return this.connected;
        }

        public abstract bool supportsBacklight();

        public abstract void setBacklight(int level);
    }
}
