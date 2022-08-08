using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace OBSCaster {
    class OBSHandler : OutputHandler {
        private OBSWebsocket obs;

        public OBSHandler() {
            obs = new OBSWebsocket();
        }

        public override bool connect(string ip, int port, string password) {
            string uri = $"ws://{ip}:{port}";
            try {
                obs.Connect(uri, password);
                return true;
            } catch (AuthFailureException) {
                MessageBox.Show("Authentication failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            } catch (ErrorResponseException err) {
                MessageBox.Show("Connect failed: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
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
                switch (type) {
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
                        obs.TransitionToProgram(transitionName: "Cut");
                        break;
                    case ConsoleEvent.TBAR:
                        // TODO: Work here required to make TBar work properly with OBS
                        double pos = value / 255.0;
                        obs.SetTBarPosition(pos);
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
    }
}
