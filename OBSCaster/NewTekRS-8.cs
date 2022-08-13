using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBSCaster {
	class NewTekRS_8: NewtekController {
		private SerialPort port;
		private int[] buttonData = { 0,  0xff , 0xff , 0xff , 0x00, 0x00, 0x00, 0x00};
		private int[] ledData = { 0xff, 0xff, 0xff };
		private bool tBarLastAt255 = false;

		private static Dictionary<int, string> OtherButtonLookup = new Dictionary<int, string>() {
			{1, "FadeDSK" },
			{2, "TakeDSK" },
			{3, "DDR" },
			{4, "Alt" },
			{5, "Auto" },
			{7, "Take" }
		};

		public NewTekRS_8() {
			Console.WriteLine("Created new NewTekRS_8 instance");
		}

		public static string deviceName() {
			return "Newtek RS-8";
        }

		protected override void _connect(string serialPortName) {
			port = new SerialPort(serialPortName);
			port.BaudRate = 115200;
			port.NewLine = "\r";
			port.DataReceived += new SerialDataReceivedEventHandler(dataReceivedHandler);
			port.Open();
		}

		protected override void _disconnect() {
			if (port.IsOpen) port.Close();
		}

		public override bool supportsBacklight() {
			return false;
		}

		public override void setBacklight(int level) {
			throw new NotImplementedException();
		}

		private void dataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
			SerialPort sp = (SerialPort)sender;
			string data = sp.ReadExisting();
			//Console.WriteLine($"Data received: {data}");
			var commands = data.Split('\r');
			foreach (var command in commands) {
				if (String.IsNullOrWhiteSpace(command)) continue;
				//Console.WriteLine($"Command: {command}");
				decodeCommand(command.Trim());
			}
		}

		public void doSomething(string button) {
			Console.WriteLine($"Button: {button}");
		}

		private void decodeCommand(string command) {
			if (command.Length != 4) {
				Console.WriteLine($"Invalid command from RS-8: {command}");
				return;
			}
			int bank = int.Parse(command.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
			int buttons = int.Parse(command.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			if (bank > 7 || bank < 1) {
				Console.WriteLine($"Invalid button bank {bank}");
				return;
			}
			if (bank == 1 || bank == 2 || bank == 3) {
				// Button decoding
				int changedButtons = buttons ^ buttonData[bank];
				for (int i = 0; i < 8; i++) {
					int bit = (1 << i);
					if ((~buttons & changedButtons & bit) > 0) {
						//Console.WriteLine($"Bank {bank} Button {8 - i} pressed");
						if (bank == 1) {
							doSomething($"Pvw{8 - i}");
						} else if (bank == 2) {
							doSomething($"Pgm{8 - i}");
						} else if (bank == 3) {
							string b;
							if (OtherButtonLookup.TryGetValue(8 - i, out b)) {
								doSomething(b);
							}
						}
					}
				}
			}
			if (bank==4) {
				// T Bar
				Console.WriteLine($"T-bar position: {buttons}");
				if (buttons==255 || buttons == 0) {
					setOtherLED(Leds.UpT, false);
					setOtherLED(Leds.DownT, false);
				} else if (buttonData[4]==255 && buttons<255) {
					setOtherLED(Leds.UpT, true);
					setOtherLED(Leds.DownT, false);
				} else if (buttonData[4]==0 && buttons > 0) {
					setOtherLED(Leds.UpT, false);
					setOtherLED(Leds.DownT, true);
				}
			}
			if (bank>4) {
				// Rotary encoder
				int change = buttons - buttonData[bank];
				if (change == 255) change = -1;
				if (change == -255) change = 1;
				// TODO: figure out if overflow occured
				Console.WriteLine($"Rotary encoder {bank} value changed by {change}");
			}

			buttonData[bank] = buttons;
		}

		public void setLED(int led, int page, bool exclusive=true, bool state=true) {
			var ledDataIndex = page - 1; // page input is 1 indexed
			led = 8-led; // led input is 1 indexed, output is flipped
			if (exclusive) {
				ledData[ledDataIndex] = (~(1 << led)) & 0xff;
			} else {
				if (state) {
					ledData[ledDataIndex] &= (~(1 << led)) & 0xff;
				} else {
					ledData[ledDataIndex] |= (1 << led) & 0xff;
				}
			}
			port.WriteLine($"~{page:X1}{ledData[ledDataIndex]:X2}");
			Console.WriteLine($"~{page:X1}{ledData[ledDataIndex]:X2}");
		}
		public override void setLedProgram(int led, bool exclusive=true) {
			setLED(led, 2, exclusive);
		}

		public override void setLedPreview(int led, bool exclusive = true) {
			setLED(led, 1, exclusive);
		}

		public override void setTransitionsLeds(bool state) {
			
		}

		public void setOtherLED(Leds led, bool state) {
			setLED((int)led, 3, false, state);
		}

		public void vegasStart() {
			new Thread(() => {
				Thread.CurrentThread.IsBackground = true;
				port.WriteLine($"~1FF");
				port.WriteLine($"~2FF");
				port.WriteLine($"~3FF");
				Thread.Sleep(50);
				port.WriteLine($"~17F");
				port.WriteLine($"~27F");
				Thread.Sleep(50);
				port.WriteLine($"~13F");
				port.WriteLine($"~23F");
				Thread.Sleep(50);
				port.WriteLine($"~11F");
				port.WriteLine($"~21F");
				Thread.Sleep(50);
				port.WriteLine($"~10F");
				port.WriteLine($"~20F");
				Thread.Sleep(50);
				port.WriteLine($"~107");
				port.WriteLine($"~207");
				Thread.Sleep(50);
				port.WriteLine($"~103");
				port.WriteLine($"~203");
				Thread.Sleep(50);
				port.WriteLine($"~101");
				port.WriteLine($"~201");
				Thread.Sleep(50);
				port.WriteLine($"~100");
				port.WriteLine($"~200");
				Thread.Sleep(50);
				port.WriteLine($"~3f8");
				Thread.Sleep(50);
				port.WriteLine($"~378");
				Thread.Sleep(50);
				port.WriteLine($"~370");
				Thread.Sleep(50);
				port.WriteLine($"~310");
				Thread.Sleep(50);
				port.WriteLine($"~300");
				Thread.Sleep(50);
				port.WriteLine($"~180");
				port.WriteLine($"~280");
				Thread.Sleep(50);
				port.WriteLine($"~1c0");
				port.WriteLine($"~2c0");
				Thread.Sleep(50);
				port.WriteLine($"~1e0");
				port.WriteLine($"~2e0");
				Thread.Sleep(50);
				port.WriteLine($"~1f0");
				port.WriteLine($"~2f0");
				Thread.Sleep(50);
				port.WriteLine($"~1f8");
				port.WriteLine($"~2f8");
				Thread.Sleep(50);
				port.WriteLine($"~1fc");
				port.WriteLine($"~2fc");
				Thread.Sleep(50);
				port.WriteLine($"~1fe");
				port.WriteLine($"~2fe");
				Thread.Sleep(50);
				port.WriteLine($"~1ff");
				port.WriteLine($"~2ff");
				Thread.Sleep(50);
				port.WriteLine($"~307");
				Thread.Sleep(50);
				port.WriteLine($"~387");
				Thread.Sleep(50);
				port.WriteLine($"~3c7");
				Thread.Sleep(50);
				port.WriteLine($"~3c7");
				Thread.Sleep(50);
				port.WriteLine($"~3cf");
				Thread.Sleep(50);
				port.WriteLine($"~3ff");
			}).Start();
		}

		public enum Leds {
			FadeDSK = 1,
			TakeDSK = 2,
			DDR = 3,
			ALT = 4,
			DownT = 6,
			UpT = 8
		}
	}
}
