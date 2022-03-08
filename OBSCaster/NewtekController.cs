using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSCaster {
    abstract class NewtekController {
        protected bool connected = false;

        // Connect to serial port
        public abstract void connect();

        // Disconnect from serial port
        public abstract void disconnect();

        public bool IsConnected() {
            return this.connected;
        }

        public abstract bool supportsBacklight();

        public abstract void setBacklight(int level);
    }
}
