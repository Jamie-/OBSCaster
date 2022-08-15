using System;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace OBSCaster {

    public class NoSerialPortsFound : Exception {

    }

    abstract class NewtekController {
        protected bool connected = false;
        private SerialPort port;
        private int baudRate;
        protected OutputHandler handler;
        private Thread readThread;
        private Thread writeThread;
        private bool _runSerialThreads;
        private char[] readBuff;
        private int readBuffPointer;
        private ConcurrentQueue<string> writeQueue;
        private EventWaitHandle wakeWrite;

        protected NewtekController(int baudRate) {
            this.baudRate = baudRate;
            writeQueue = new ConcurrentQueue<string>();
        }

        // Connect to serial port
        public void connect() {
            Console.WriteLine("Connecting to controller...");
            List<string> ports = SerialPortModel.ComPortFromIDs("0403", "6001");
            if (ports.Count > 0) {
                if (ports.Count > 1) {
                    Console.WriteLine("WARNING: More than one serial port for IDs found!");
                }
                port = new SerialPort(ports[0]);
                port.BaudRate = baudRate;
                port.ReadTimeout = 500;
                port.WriteTimeout = 500;
                port.Open();
                this._connect();
                this.connected = true;
                Console.WriteLine("Controller connected!");
            } else {
                throw new NoSerialPortsFound();
            }
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

        // Internal workings of connection
        protected abstract void _connect();

        // Disconnect from serial port
        public void disconnect() {
            Console.WriteLine("Disconnecting from controller...");
            this._disconnect();
            // Close internal threads
            _runSerialThreads = false;
            readThread.Join();
            writeThread.Join();
            port.Close();
            this.connected = false;
            Console.WriteLine("Controller disconnected!");
        }

        // Internal workings of disconnection
        protected abstract void _disconnect();

        public bool IsConnected() {
            return this.connected;
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
                        readBuff[readBuffPointer] = (char)chunk;
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

        // Queue a write for the write thread
        protected void queueWrite(string cmd) {
            writeQueue.Enqueue(cmd);
            wakeWrite.Set();
        }

        public abstract bool supportsBacklight();

        public abstract void setBacklight(int level);

        protected abstract void decodeCommand(string command);

        public abstract void setLedProgram(int idx, bool exclusive = true);

        public abstract void setLedPreview(int idx, bool exclusive = true);

        public abstract void setTransitionsLeds(bool state);
    }
}
