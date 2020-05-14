using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace CameraBasler.ViewModel
{
    public class ArduinoViewModel : ViewModel
    {
        private SerialPort port;

        private string selectedPort;
        private string command;
        private string recivedData;

        public string SelectedPort
        {
            get => selectedPort;
            set
            {
                selectedPort = value;
                OnPropertyChanged();
            }
        }

        public string Command
        {
            get => command;
            set
            {
                command = value;
                OnPropertyChanged();
            }
        }

        public string RecivedData
        {
            get => recivedData;
            set
            {
                recivedData = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> AvailablePorts
        {
            get => SerialPort.GetPortNames();
            set => OnPropertyChanged();
        }

        public bool IsPortOpen => port == null ? false : port.IsOpen;

        public void RefreshPorts()
        {
            OnPropertyChanged(nameof(AvailablePorts));
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(SelectedPort)) 
            {
                return;
            }

            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += SelectedPort_DataReceived;
            port.Open();
            port.Write("#");
            port.DtrEnable = true;
        }

        public void Disconnect()
        {
            if (port == null)
            {
                return;
            }

            port.Write("#STOP");
            port.DataReceived -= SelectedPort_DataReceived;
            port.Close();
            port = null;
        }

        public void WriteCommand()
        {
            if (port == null)
            {
                return;
            }

            port.Write(Command);
        }

        private void SelectedPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RecivedData = port.ReadLine();
        }
    }
}
