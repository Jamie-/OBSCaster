using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSCaster {
    class NewtekMiniController : NewtekController {
        public static string deviceName() {
            return "Newtek Mini Control Surface";
        }

        public override void connect() {

            this.connected = true;
        }

        public override void disconnect() {

            this.connected = false;
        }

        public override bool supportsBacklight() {
            return true;
        }

        // Set backlight
        public override void setBacklight(int level) {

        }
    }
}
