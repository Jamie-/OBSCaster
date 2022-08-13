using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace OBSCaster {
    class NewtekMiniController : NewtekController {
        private SerialPort port;
        private Thread readThread;
        private Thread writeThread;
        private bool _runSerialThreads;
        private char[] readBuff;
        private int readBuffPointer;
        private int[] bankState;
        OutputHandler handler;
        private ConcurrentQueue<string> writeQueue;
        private EventWaitHandle wakeWrite;
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

        public NewtekMiniController(OutputHandler handler) {
            // Console.WriteLine("Created new NewtekMiniController instance");
            this.handler = handler;
            writeQueue = new ConcurrentQueue<string>();
        }

        public static string deviceName() {
            return "Newtek Mini Control Surface";
        }

        protected override void _connect(string serialPortName) {
            port = new SerialPort(serialPortName);
            port.BaudRate = 9600;
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            port.Open();
            bankState = new int[] { 0, 0, 0, 0, 0, 0, 19 };
            readThread = new Thread(readSerialPort);
            writeThread = new Thread(writeSerialPort);
            _runSerialThreads = true;
            readBuff = new char[4];
            readBuffPointer = 0;
            // TODO: implement alternative to .Clear()
            // .Clear() only added in .NET 5
            // writeQueue.Clear();
            wakeWrite = new EventWaitHandle(false, EventResetMode.ManualReset);
            readThread.Start();
            writeThread.Start();
        }

        // Internal serial read thread
        private void readSerialPort() {
            Console.WriteLine("Starting serial read thread...");
            while (_runSerialThreads) {
                try {
                    Int32 chunk = port.ReadChar();
                    if (readBuffPointer == 4) {
                        if (chunk == 13) {
                            // Dispatch event
                            string cmd = new string(readBuff);
                            decodeCommand(cmd);
                        } else {
                            // Got something weird - just reset the pointer and move on
                            Console.WriteLine("Received malformed data!");
                        }
                        readBuffPointer = 0;
                    } else {
                        readBuff[readBuffPointer] = (char) chunk;
                        readBuffPointer++;
                    }
                } catch (TimeoutException) { }
            }
            Console.WriteLine("Serial read thread exited!");
        }

        // Internal serial write thread
        private void writeSerialPort() {
            Console.WriteLine("Starting serial write thread...");
            while (_runSerialThreads) {
                string data;
                if (writeQueue.TryDequeue(out data)) {
                    port.Write(data + "\r");
                } else {
                    if (wakeWrite.WaitOne(500)) {
                        // Reset the event ready for the next iteration
                        wakeWrite.Reset();
                    }
                }
            }
            Console.WriteLine("Serial write thread exited!");
        }

        // Close internal read thread and serial port
        protected override void _disconnect() {
            _runSerialThreads = false;
            readThread.Join();
            writeThread.Join();
            port.Close();
        }

        // Queue a write for the write thread
        private void _queueWrite(string cmd) {
            writeQueue.Enqueue(cmd);
            wakeWrite.Set();
        }

        public override bool supportsBacklight() {
            return true;
        }

        // Set backlight
        public override void setBacklight(int level) {
            Console.WriteLine($"Setting backlight to level: {level}");
            Debug.Assert(level >= 0 && level <= 7);
            port.Write($"070{level}\r");
        }

        private void decodeCommand(string command) {
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
                _queueWrite($"11" + val.ToString("X"));
                _queueWrite($"14FF");
            } else {
                // Second bank
                int val = 255 & ~(1 << idx - 8);
                _queueWrite($"14" + val.ToString("X"));
                _queueWrite($"11FF");
            }
        }

        // Set the preview LED to the given index
        public override void setLedPreview(int idx, bool exclusive = true) {
            if (idx < 8) {
                // First bank
                int val = 255 & ~(1 << idx);
                _queueWrite($"12" + val.ToString("X"));
                _queueWrite($"133F");
            } else {
                // Second bank (192 causes take & auto to stay lit)
                int val = 255 & ~((1 << idx - 8) | 192);
                _queueWrite($"13" + val.ToString("X"));
                _queueWrite($"12FF");
            }
        }
    }
}
