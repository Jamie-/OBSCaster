using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OBSCaster {
	class Vmix {
		private Timer statusTimer;
		private NewTekRS_8 controller;

		private static int preview, program, convertedPreview, convertedProgram;
		private static bool ftb, recording, streaming, overlay1, overlay2;

		public static Dictionary<int, int> incomingInputLookup = new Dictionary<int, int>() {
			{2, 1}, // Mark
			{3, 2}, // Kim Trott Rhi
			{4, 3 }, // Katie Tom
			{5,4 }, // Main
			{29, 5}, // Main with map
			{6, 6 }, // Map Focus
			{7, 7 }, // intro
			{13, 8 } // fan art
		};

		public static Dictionary<int, int> outgoingInputLookup = new Dictionary<int, int>() {
			{1, 2 },
			{2, 3 },
			{3, 4 },
			{4, 5 },
			{5, 29 },
			{6, 6 },
			{7, 100 }, // special
			{8, 101 }
		};


		public Vmix(NewTekRS_8 c) {
			statusTimer = new Timer(new TimerCallback(getStatus), null, 0, 500);
			controller = c;
		}
		public void getStatus(object stateInfo=null) {
			try {
				var xml = XDocument.Load(@"http://192.168.1.2:8088/api/");
				preview = int.Parse(xml.Root.Descendants("preview").First().Value);
				program = int.Parse(xml.Root.Descendants("active").First().Value);
				ftb = bool.Parse(xml.Root.Descendants("fadeToBlack").First().Value);
				recording = bool.Parse(xml.Root.Descendants("recording").First().Value);
				streaming = bool.Parse(xml.Root.Descendants("streaming").First().Value);

				var overlayNodes = xml.Root.Descendants("overlays").Descendants("overlay");
				overlay1 = !String.IsNullOrEmpty(overlayNodes.ElementAt(0).Value);
				overlay2 = !String.IsNullOrEmpty(overlayNodes.ElementAt(1).Value);

				convertedPreview = 0;
				incomingInputLookup.TryGetValue(preview, out convertedPreview);
				convertedProgram = 0;
				incomingInputLookup.TryGetValue(program, out convertedProgram);
				controller.setPreviewLED(convertedPreview);
				controller.setProgramLED(convertedProgram);
				controller.setOtherLED(NewTekRS_8.Leds.DDR, overlay1);
				controller.setOtherLED(NewTekRS_8.Leds.ALT, overlay2);
				controller.setOtherLED(NewTekRS_8.Leds.UpT, streaming);

				if (program == 22) ftb = true;
				controller.setOtherLED(NewTekRS_8.Leds.DownT, ftb);

				Console.WriteLine($"Preview: {preview}, Program: {program}");
			} catch (Exception) {
				Console.WriteLine($"XML Exception on get Vmix Status");
			}
		}

		public static void setProgram(int program) {
			if (program != 5 && program != 29) {
				if (overlay1) sendvmix("OverlayInput1Off", 0);
				if (overlay2) sendvmix("OverlayInput2Off", 0);
			}
			sendvmix("ActiveInput", program);
		}

		public static void setPreview(int preview) {
			sendvmix("PreviewInput", preview);
		}

		public static void setTBar(int value) {
			sendvmixvalue("SetFader", value);
		}

		public static void fadeToBlack() {
			if (overlay1) sendvmix("OverlayInput1Off", 0);
			if (overlay2) sendvmix("OverlayInput2Off", 0);
			sendvmix("FadeToBlack", 0);
		}

		public static void cut() {
			if (preview != 5 && preview != 29) {
				if (overlay1) sendvmix("OverlayInput1Off", 0);
				if (overlay2) sendvmix("OverlayInput2Off", 0);
			}
			if ((preview == 5 && program == 29) || (preview == 29 && program == 5)) {
				sendvmix("Transition2",0);
			} else {
				sendvmix("Cut", 0);
			}
		}

		public static void fade() {
			if (preview != 5 && preview != 29) {
				if (overlay1) sendvmix("OverlayInput1Out", 0);
				if (overlay2) sendvmix("OverlayInput2Out", 0);
			}
			sendvmix("Transition2", 0);
		}

		public static void setoverlay1() {
			sendvmix("OverlayInput1", 23);
		}

		public static void setoverlay2() {
			sendvmix("OverlayInput2", 15);
		}

		public static void bodge(int input, int source=0) {
			switch(input) {
				case 100:
					if (source==0) {
						// ftb
						if (overlay1) sendvmix("OverlayInput1Out", 0);
						if (overlay2) sendvmix("OverlayInput2Out", 0);
						sendcompanion(12, 3);
					} else if (source==1) {
						// into
						sendcompanion(12, 20);
					}
					break;
				case 101:
					if (source==0) {
						// ftb + pause
						if (overlay1) sendvmix("OverlayInput1Out", 0);
						if (overlay2) sendvmix("OverlayInput2Out", 0);
						sendcompanion(12, 19);
					} else if (source==1) {
						// fan art
						sendcompanion(12, 13);
					}
					break;
				default:
					break;
			}
		}

		public static void sendvmix(string function, int input) {
			var request = WebRequest.Create($"http://192.168.1.2:8088/api/?Function={function}&Input={input}");
			request.Method = "GET";
			request.Timeout = 100;
			try {
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Console.WriteLine($"{response.StatusDescription} http://192.168.1.2:8088/api/?Function={function}&Input={input}");
				response.Close();
			} catch (WebException) {
				Console.WriteLine($"Timeout http://192.168.1.2:8088/api/?Function={function}&Input={input}");
			}
		}

		public static void sendvmixvalue(string function, int value) {
			var request = WebRequest.Create($"http://192.168.1.2:8088/api/?Function={function}&Value={value}");
			request.Method = "GET";
			request.Timeout = 100;
			try {
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Console.WriteLine($"{response.StatusDescription} http://192.168.1.2:8088/api/?Function={function}&Value={value}");
				response.Close();
			} catch (WebException) {
				Console.WriteLine($"Timeout http://192.168.1.2:8088/api/?Function={function}&Value={value}");
			}
		}

		public static void sendcompanion(int page, int bank) {
			var request = WebRequest.Create($"http://192.168.1.2:8888/press/bank/{page}/{bank}");
			request.Method = "GET";
			request.Timeout = 100;
			try {
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Console.WriteLine($"{response.StatusDescription} http://192.168.1.2:8888/press/bank/{page}/{bank}");
				response.Close();
			} catch (WebException) {
				Console.WriteLine($"Timeout http://192.168.1.2:8888/press/bank/{page}/{bank}");
			}
		}
	}
}
