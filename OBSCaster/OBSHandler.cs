using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.Threading;
using System.Linq;
using Loupedeck.SymetrixPlugin;
using System.Diagnostics;

namespace OBSCaster {
    class OBSHandler : OutputHandler {
        private OBSWebsocket obs;
        // Cache of string names (to calculate indexes)
        private List<string> sceneList;
        // Knobs state
        private int[] knobState = new int[] { 0, 0, 0 };

        private SymetrixInterface symetrix;
        private Thread symetrixThread;
        private bool symetrixThreadRun = true;

        private bool fadeDSKIndicator = false;
        private bool takeDSKIndicator = false;
        private bool DDRIndicator = false;
        private bool ALTIndicator = false;

        private int lastKnob1 = 0;
        private int lastKnob2 = 0;
        private int lastKnob3 = 0;

        public OBSHandler() {
            obs = new OBSWebsocket();
            sceneList = new List<string>();
            obs.Connected += onConnected;
            obs.SceneChanged += onSceneChange;
            obs.PreviewSceneChanged += onPreviewSceneChange;
            obs.SceneListChanged += onScenesChanged;
            obs.TransitionBegin += onTransitionBegin;
            obs.TransitionEnd += onTransitionEnd;

            this.symetrix = new SymetrixInterface();
            this.symetrix.connect();

            this.symetrixThread = new Thread(this.symextrixRXThread);
            this.symetrixThread.Start();
        }

        private void symextrixRXThread() {
            Debug.WriteLine("Starting Symetrix RX Thread");
			while (symetrixThreadRun) {
                Thread.Sleep(500);
                fadeDSKIndicator = symetrix.getControl(1) > 0 ? true : false;
				takeDSKIndicator = symetrix.getControl(2) > 0 ? true : false;
				DDRIndicator = symetrix.getControl(3) > 0 ? true : false;
				ALTIndicator = symetrix.getControl(4) > 0 ? true : false;

				NewTekRS_8 rs8 = (NewTekRS_8)this.controller;
				rs8.setOtherLED(NewTekRS_8.Leds.FadeDSK, !fadeDSKIndicator);
				rs8.setOtherLED(NewTekRS_8.Leds.TakeDSK, !takeDSKIndicator);
				rs8.setOtherLED(NewTekRS_8.Leds.DDR, !DDRIndicator);
				rs8.setOtherLED(NewTekRS_8.Leds.ALT, !ALTIndicator);

                if (obs.IsConnected) {
                    var obsStatus = obs.GetStreamingStatus();
                    rs8.setOtherLED(NewTekRS_8.Leds.UpT, obsStatus.IsStreaming);
					rs8.setOtherLED(NewTekRS_8.Leds.DownT, obsStatus.IsRecording);
				}
            }
            Debug.WriteLine("Symetrix RX Thread exiting");
        }

        public override bool connect(string ip, int port, string password) {
            string uri = $"ws://{ip}:{port}";
            try {
                obs.Connect(uri, password);
                if (!obs.WSConnection.IsAlive) {
                    Console.WriteLine("OBS websocket connection failed!");
                    MessageBox.Show(
                        "Connection to OBS failed\nHave you enabled the websocket?",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
                    return false;
                }
                return true;
            } catch (AuthFailureException) {
                Console.WriteLine("OBS websocket auth failed!");
                MessageBox.Show("Authentication failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } catch (ErrorResponseException err) {
                Console.WriteLine($"OBS websocket returned error! {err.Message}");
                MessageBox.Show("Connect failed: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

        public override void disconnect() {
            obs.Disconnect();
            this.symetrixThreadRun = false;
        }

        // Update the sceneList cache
        private void updateSceneList() {
            List<OBSScene> scenes = obs.ListScenes();
            sceneList = scenes.Select(o => o.Name).ToList();
        }

        // Dispatch event to OBS
        public override void dispatchEvent(ConsoleEvent type, int value = -1) {
            if (value >= 0) {
                Console.WriteLine($"{type.ToString()}: {value}");
            } else {
                Console.WriteLine(type.ToString());
            }
            try {
                int delta = 0;
                switch (type) {
                    case ConsoleEvent.TBAR:
                        bool release = value == 0 || value == 255;
                        double pos = value / 255.0;
                        obs.SetTBarPosition(pos, release);
                        break;
                    case ConsoleEvent.PREVIEW:
                        changeBusScene(type, value);
                        break;
                    case ConsoleEvent.PROGRAM:
                        changeBusScene(type, value);
                        break;
                    case ConsoleEvent.AUTO:
                        obs.TransitionToProgram();
                        break;
                    case ConsoleEvent.TAKE:
						obs.TransitionToProgram();
						break;
						TransitionSettings oldTransition = obs.GetCurrentTransition();
                        obs.TransitionToProgram(transitionName: "Cut");
                        Thread.Sleep(50);  // OBS needs at least 1 frame between cut and change
                        obs.SetCurrentTransition(oldTransition.Name);
                        break;
                    case ConsoleEvent.KNOB1:
                        // Change selected transition
                        //delta = (value + 128) % 256 - (knobState[0] + 128) % 256;
                        //var delta2 = delta - lastKnob1;
                        //this.symetrix.setControlRelative(4,Math.Abs(delta2 * -100),(delta2 > 0) ? true : false);
                        //Console.WriteLine(delta2);
                        //lastKnob1 = delta;
                        break;
                    case ConsoleEvent.KNOB2:
                        // Change selected transition duration
      //                  delta = (value + 128) % 256 - (knobState[1] + 128) % 256;
						//var delta2_ = delta - lastKnob2;
						//this.symetrix.setControlRelative(5, Math.Abs(delta2_ * -100), (delta2_ > 0) ? true : false);
						//this.symetrix.setControlRelative(6, Math.Abs(delta2_ * -100), (delta2_ > 0) ? true : false);
      //                  Console.WriteLine(delta2_);
      //                  lastKnob2 = delta;
						break;
                    case ConsoleEvent.FADE_DSK:
                        fadeDSKIndicator = !fadeDSKIndicator;
                        this.symetrix.setControl(1, fadeDSKIndicator ? 65535 : 0);
                        break;
                    case ConsoleEvent.TAKE_DSK:
						takeDSKIndicator = !takeDSKIndicator;
						this.symetrix.setControl(2, takeDSKIndicator ? 65535 : 0);
						break;
                    case ConsoleEvent.DDR:
						DDRIndicator = !DDRIndicator;
						this.symetrix.setControl(3, DDRIndicator ? 65535 : 0);
						break;
                    case ConsoleEvent.ALT:
						ALTIndicator = !ALTIndicator;
						this.symetrix.setControl(1, ALTIndicator ? 65535 : 0);
						break;
				}
            } catch (ErrorResponseException err) {
                Console.WriteLine($"ERROR FROM OBS: {err.Message}");
            }
        }

        // Change the scene in a bus
        private void changeBusScene(ConsoleEvent bus, int index) {
            updateSceneList();
            if (index > sceneList.Count) {
                // No-op for now if the selected scene doesn't exist
                Console.WriteLine("Selected scene doesn't exist, no-op");
                return;
            }
            string scene = sceneList[index - 1];
            if (bus == ConsoleEvent.PROGRAM) {
                Console.WriteLine($"Setting PROGRAM to {scene}");
                obs.SetCurrentScene(scene);
            } else if (bus == ConsoleEvent.PREVIEW) {
                Console.WriteLine($"Setting PREVIEW to {scene}");
                obs.SetPreviewScene(scene);
            }
        }

        // Changes the selected transition
        private void incrSelectedTransition(int delta) {
            GetTransitionListInfo transitions = obs.GetTransitionList();
            int idx = transitions.Transitions.FindIndex(t => t.Name == transitions.CurrentTransition);
            idx += delta;
            while (idx < 0) {
                idx += transitions.Transitions.Count;
            }
            idx %= transitions.Transitions.Count;
            obs.SetCurrentTransition(transitions.Transitions[idx].Name);
        }

        // Changes the selected transition duration
        private void incrTransitionDuration(int delta) {
            int duration = obs.GetTransitionDuration();
            duration += delta * 100;
            if (duration < 100) duration = 100;
            obs.SetTransitionDuration(duration);
        }

        // Set initial controller LEDs on first connect
        private void onConnected(object sender, EventArgs e) {
            controller.setTransitionsLeds(true);
            updateSceneList();
            controller.setLedProgram(sceneList.IndexOf(obs.GetCurrentScene().Name));
            controller.setLedPreview(sceneList.IndexOf(obs.GetPreviewScene().Name));
        }

        private void onSceneChange(OBSWebsocket sender, string newSceneName) {
            Console.WriteLine($"Scene change: {newSceneName}");
            updateSceneList();
            controller.setLedProgram(sceneList.IndexOf(newSceneName));
        }

        private void onPreviewSceneChange(OBSWebsocket sender, string newSceneName) {
            Console.WriteLine($"Preview scene change: {newSceneName}");
            updateSceneList();
            controller.setLedPreview(sceneList.IndexOf(newSceneName));
        }

        private void onScenesChanged(object sender, EventArgs e) {
            updateSceneList();
        }

        private void onTransitionBegin(OBSWebsocket sender, string transitionName, string transitionType, int duration, string fromScene, string toScene) {
            Console.WriteLine($"Starting transition {transitionName}");
            if (transitionType != "cut_transition") controller.setTransitionsLeds(false);
        }

        private void onTransitionEnd(OBSWebsocket sender, string transitionName, string transitionType, int duration, string toScene) {
            Console.WriteLine($"Ended transition {transitionName}");
            controller.setTransitionsLeds(true);
        }
    }
}
