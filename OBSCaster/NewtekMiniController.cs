﻿using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OBSCaster {
    class NewtekMiniController : NewtekController {
        private int[] bankState;
        private int[] ledState;
        private bool flipTbar = false;
        private int tBarLedState = 0;
        private static readonly Dictionary<int, (ConsoleEvent, int)> buttonLookup = new Dictionary<int, (ConsoleEvent, int)>() {
            {3*8+0, (ConsoleEvent.PREVIEW, 9)},
            {3*8+1, (ConsoleEvent.PREVIEW, 10)},
            {3*8+2, (ConsoleEvent.PREVIEW, 11)},
            {3*8+3, (ConsoleEvent.PREVIEW, 12)},
            {3*8+4, (ConsoleEvent.PREVIEW, 13)},
            {3*8+5, (ConsoleEvent.PREVIEW, 14)},
            {3*8+6, (ConsoleEvent.TAKE, -1)},
            {3*8+7, (ConsoleEvent.AUTO, -1)},
            {4*8+0, (ConsoleEvent.PROGRAM, 9)},
            {4*8+1, (ConsoleEvent.PROGRAM, 10)},
            {4*8+2, (ConsoleEvent.PROGRAM, 11)},
            {4*8+3, (ConsoleEvent.PROGRAM, 12)},
            {4*8+4, (ConsoleEvent.PROGRAM, 13)},
            {4*8+5, (ConsoleEvent.PROGRAM, 14)},
            {4*8+6, (ConsoleEvent.AUX, 9)},
            {4*8+7, (ConsoleEvent.AUX, 10)},
            {5*8+0, (ConsoleEvent.ME, 1)},
            {5*8+1, (ConsoleEvent.ME, 2)},
            {5*8+2, (ConsoleEvent.ME, 3)},
            {5*8+3, (ConsoleEvent.ME, 4)},
            {5*8+4, (ConsoleEvent.SHIFT, -1)},
            {5*8+5, (ConsoleEvent.ALT, -1)},
            {5*8+6, (ConsoleEvent.DSK, 1)},
            {5*8+7, (ConsoleEvent.DSK, 2)},
            {6*8+2, (ConsoleEvent.MAIN, -1)},
            {6*8+3, (ConsoleEvent.KNOBBUTTON, 1)},
            {6*8+5, (ConsoleEvent.KNOBBUTTON, 2)},
            {6*8+6, (ConsoleEvent.BKGD, -1)},
            {6*8+7, (ConsoleEvent.FTB, -1)},
        };

        public NewtekMiniController(OutputHandler handler):base(9600) {
            this.handler = handler;
        }

        public static string deviceName() {
            return "Newtek Mini Control Surface";
        }

        protected override void _connect() {
            bankState = new int[] { 0, 0, 0, 0, 0, 0, 19 };
            ledState = new int[] { 255, 255, 255, 63, 255, 255, 255 };
        }

        protected override void _disconnect() { }

        public override bool supportsBacklight() {
            return true;
        }

        // Set backlight
        public override void setBacklight(int level) {
            Console.WriteLine($"Setting backlight to level: {level}");
            Debug.Assert(level >= 0 && level <= 7);
            queueWrite($"070{level}\r");
        }

        protected override void decodeCommand(string command) {
            if (command.Length != 4) {
                Console.WriteLine($"Invalid command from controller: {command}");
                return;
            }
            int bank = int.Parse(command.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int value = int.Parse(command.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            // Console.WriteLine($"Bank: {bank}, Value: {value}");
            if (bank >= 16 && bank < 24) {
                // Buttons
                // 16: Aux 1-8
                // 17: Program 1-8
                // 18: Preview 1-8
                // 19: Preview 9-14, take, auto
                // 20: Program 9-14, Aux 9-10
                // 21: Shift, Alt, DSK 1-2, M/E 1-4
                // 23: Main, BKGD, FTB, Knob 1-2
                int bankIdx = bank - 16;
                if (bank == 23) bankIdx = 6;
                dispatchDecodeValue(bankIdx, value);
                return;
            }
            switch (bank) {
                case 24:
                    handler.dispatchEvent(ConsoleEvent.KNOB2, value);
                    break;
                case 25:
                    handler.dispatchEvent(ConsoleEvent.KNOB1, value);
                    break;
                case 128:
                    // Set TBar directional LEDs
                    if (value == 0 || value == 255) setTBarLeds(0);
                    else if (flipTbar) setTBarLeds(1);
                    else setTBarLeds(2);
                    // Flip TBar direction to always count 0-255
                    if (flipTbar) value = 255 - value;
                    // If we abort the transition, turn transition LEDs back on
                    if (value == 0) setTransitionsLeds(true);
                    if (value == 255) flipTbar = !flipTbar;
                    handler.dispatchEvent(ConsoleEvent.TBAR, value);
                    break;
                default:
                    Console.WriteLine("UNKNOWN BANK!");
                    break;
            }
        }

        private void dispatchDecodeValue(int bankIdx, int value) {
            int inverseVal = ~value;
            int inverseBS = ~bankState[bankIdx];
            for (int i = 0; i < 8; i++) {
                if ((inverseVal & (1 << i) & inverseBS) > 0) {
                    // Console.WriteLine($"Decoded value: {i}");
                    switch (bankIdx) {
                        case 0:
                            handler.dispatchEvent(ConsoleEvent.AUX, i+1);
                            break;
                        case 1:
                            handler.dispatchEvent(ConsoleEvent.PROGRAM, i+1);
                            break;
                        case 2:
                            handler.dispatchEvent(ConsoleEvent.PREVIEW, i+1);
                            break;
                        default:
                            int key = bankIdx * 8 + i;
                            Debug.Assert(buttonLookup.ContainsKey(key));
                            (ConsoleEvent type, int val) = buttonLookup[key];
                            handler.dispatchEvent(type, val);
                            break;
                    }
                }
            }
            bankState[bankIdx] = inverseVal;
        }

        // Set the program LED to the given index
        public override void setLedProgram(int idx, bool exclusive = true) {
            if (idx < 8) {
                // First bank
                int val = 255 & ~(1 << idx);
                queueWrite("11" + val.ToString("X2"));
                queueWrite("14FF");
            } else {
                // Second bank
                int val = 255 & ~(1 << idx - 8);
                queueWrite("14" + val.ToString("X2"));
                queueWrite("11FF");
            }
        }

        // Set the preview LED to the given index
        public override void setLedPreview(int idx, bool exclusive = true) {
            if (idx < 8) {
                // First bank
                int val = 255 & ~(1 << idx);
                queueWrite("12" + val.ToString("X2"));
                val = ledState[3] | 63;
                queueWrite("13" + val.ToString("X2"));
                ledState[3] = val;
            } else {
                // Second bank
                int val = (ledState[3] | 63) & ~(1 << idx - 8);
                queueWrite("13" + val.ToString("X2"));
                ledState[3] = val;
                queueWrite("12FF");
            }
        }

        // Set the auto/take LEDs on and off
        public override void setTransitionsLeds(bool state) {
            if (state) {
                int val = ledState[3] & ~(3 << 6);
                queueWrite("13" + val.ToString("X2"));
                ledState[3] = val;
            } else {
                int val = ledState[3] | (3 << 6);
                queueWrite("13" + val.ToString("X2"));
                ledState[3] = val;
            }
        }

        // Set the TBar directional LEDs
        private void setTBarLeds(int state) {
            // 0: off, 1: bottom, 2: top
            if (state == tBarLedState) return;
            switch (state) {
                case 0:
                    queueWrite("09FF");
                    break;
                case 1:
                    queueWrite("09FE");
                    break;
                case 2:
                    queueWrite("09FD");
                    break;
            }
            tBarLedState = state;
        }
    }
}
