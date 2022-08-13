using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.Threading;

namespace OBSCaster {
    class OBSHandler : OutputHandler {
        private OBSWebsocket obs;
        // Knobs state
        private int[] knobState = new int[] { 0, 0, 0 };
        private bool flipTbar = false;

        public OBSHandler() {
            obs = new OBSWebsocket();
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
                        if (flipTbar) value = 255 - value;
                        if (value == 255) flipTbar = !flipTbar;
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
                        TransitionSettings oldTransition = obs.GetCurrentTransition();
                        obs.TransitionToProgram(transitionName: "Cut");
                        Thread.Sleep(50);  // OBS needs at least 1 frame between cut and change
                        obs.SetCurrentTransition(oldTransition.Name);
                        break;
                    case ConsoleEvent.KNOB1:
                        // Change selected transition
                        delta = (value + 128) % 256 - (knobState[0] + 128) % 256;
                        incrSelectedTransition(delta);
                        knobState[0] = value;
                        break;
                    case ConsoleEvent.KNOB2:
                        // Change selected transition duration
                        delta = (value + 128) % 256 - (knobState[1] + 128) % 256;
                        incrTransitionDuration(delta);
                        knobState[1] = value;
                        break;
                }
            } catch (ErrorResponseException err) {
                Console.WriteLine($"ERROR FROM OBS: {err.Message}");
            }
        }

        // Change the scene in a bus
        private void changeBusScene(ConsoleEvent bus, int index) {
            List<OBSScene> scenes = obs.ListScenes();
            if (index > scenes.Count) {
                // No-op for now if the selected scene doesn't exist
                Console.WriteLine("Selected scene doesn't exist, no-op");
                return;
            }
            OBSScene scene = scenes[index - 1];
            if (bus == ConsoleEvent.PROGRAM) {
                Console.WriteLine($"Setting PROGRAM to {scene.Name}");
                obs.SetCurrentScene(scene.Name);
            } else if (bus == ConsoleEvent.PREVIEW) {
                Console.WriteLine($"Setting PREVIEW to {scene.Name}");
                obs.SetPreviewScene(scene.Name);
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
    }
}
