using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace OBSCaster {
	class NewTekRS_8: NewtekController {
		private int[] buttonData = { 0,  0xff , 0xff , 0xff , 0x00, 0x00, 0x00, 0x00};
		private int[] ledData = { 0xff, 0xff, 0xff };
		private bool flipTbar = false;

		private static Dictionary<int, ConsoleEvent> OtherButtonLookup = new Dictionary<int, ConsoleEvent>() {
			{1, ConsoleEvent.FADE_DSK},
			{2, ConsoleEvent.TAKE_DSK},
			{3, ConsoleEvent.DDR},
			{4, ConsoleEvent.ALT},
			{5, ConsoleEvent.AUTO},
			{7, ConsoleEvent.TAKE},
		};

		public NewTekRS_8(OutputHandler handler):base(115200) {
			this.handler = handler;
		}

		public static string deviceName() {
			return "Newtek RS-8";
        }

		protected override void _connect() { }

		protected override void _disconnect() { }

		public override bool supportsBacklight() {
			return false;
		}

		public override void setBacklight(int level) {
			throw new NotImplementedException();
		}

		public void doSomething(string button) {
			Console.WriteLine($"Button: {button}");
		}

		protected override void decodeCommand(string command) {
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
							Console.WriteLine($"Pvw{8 - i}");
							handler.dispatchEvent(ConsoleEvent.PREVIEW, 8 - i);
						} else if (bank == 2) {
							Console.WriteLine($"Pgm{8 - i}");
							handler.dispatchEvent(ConsoleEvent.PROGRAM, 8 - i);
						} else if (bank == 3) {
							if (OtherButtonLookup.ContainsKey(8-i)) {
								handler.dispatchEvent(OtherButtonLookup[8 - i], -1);
							}
						}
					}
				}
			}
			if (bank==4) {
				// T Bar
				Console.WriteLine($"T-bar position: {buttons}");
				if (buttons == 0 || buttons == 255) {
					setOtherLED(Leds.UpT, false);
					setOtherLED(Leds.DownT, false);
				} else if (flipTbar) {
					setOtherLED(Leds.UpT, true);
					setOtherLED(Leds.DownT, false);
				} else {
					setOtherLED(Leds.UpT, false);
					setOtherLED(Leds.DownT, true);
				}
				// Flip TBar direction to always count 0-255
				if (flipTbar) buttons = 255 - buttons;
				if (buttons == 255) flipTbar = !flipTbar;
				handler.dispatchEvent(ConsoleEvent.TBAR, buttons);
			}
			if (bank>4) {
				// Rotary encoder
				if (bank == 5) {
					handler.dispatchEvent(ConsoleEvent.KNOB1, buttons);
				} else if (bank == 6) {
					handler.dispatchEvent(ConsoleEvent.KNOB2, buttons);
				}
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
			queueWrite($"~{page:X1}{ledData[ledDataIndex]:X2}");
			Console.WriteLine($"~{page:X1}{ledData[ledDataIndex]:X2}");
		}
		public override void setLedProgram(int led, bool exclusive=true) {
			setLED(led+1, 2, exclusive);
		}

		public override void setLedPreview(int led, bool exclusive = true) {
			setLED(led+1, 1, exclusive);
		}

		// RS-8 doesn't have transition LEDs
		public override void setTransitionsLeds(bool state) { }

		public void setOtherLED(Leds led, bool state) {
			setLED((int)led, 3, false, state);
		}

		public void vegasStart() {
			// TODO: fix this
			SerialPort port = null;
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
