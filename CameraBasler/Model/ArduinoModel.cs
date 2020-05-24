using CameraBasler.Events;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace CameraBasler.Model
{
    public class ArduinoModel
    {
        private SerialPort port;

        public bool IsOpen => port == null ? false : port.IsOpen;

        public IEnumerable<string> AvailablePorts => SerialPort.GetPortNames();

        public event EventHandler<SerialDataReceivedEventArgs> OnDataRecived;
        public event EventHandler<CommandEventArgs> OnCommandSended;

        public bool Connect(string portName)
        {
            if (string.IsNullOrEmpty(portName))
            {
                return false;
            }

            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += Port_DataReceived;
            port.Open();
            WriteCommand("#STAR");
            port.DtrEnable = true;
            return true;
        }  

        public void Disconnect()
        {
            if (port == null)
            {
                return;
            }

            port.Write("#STOP");
            port.DataReceived -= Port_DataReceived;
            port.Close();
            port = null;
        }

        public void WriteCommand(string command)
        {
            if (port == null || !port.IsOpen)
            {
                return;
            }

            ArduinoModel_OnCommandSended(this, new CommandEventArgs(command));

            port.WriteLine(command);
        }

        public string ReadPortData()
        {
            return port.ReadLine();
        }

        private void ArduinoModel_OnCommandSended(object sender, CommandEventArgs e)
        {
            OnCommandSended?.Invoke(this, e);
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnDataRecived?.Invoke(sender, e);
        }
    }
}
