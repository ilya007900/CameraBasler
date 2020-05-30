using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace CameraBasler.Model
{
    public class ArduinoModel : Model
    {
        private SerialPort port;

        public static IEnumerable<string> AvailablePorts => SerialPort.GetPortNames();

        public ObservableCollection<string> SendedCommands { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> RecivedData { get; } = new ObservableCollection<string>();

        public bool IsPortOpen => port.IsOpen;

        public ArduinoModel(string portName)
        {
            Connect(portName);
        }

        public void Connect(string portName)
        {
            if (port != null)
            {
                Disconnect();
            }

            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += Port_DataReceived;
            port.Open();
            OnPropertyChanged(nameof(IsPortOpen));
            WriteCommand("#STAR");
            port.DtrEnable = true;
        }

        public void Disconnect()
        {
            if (port == null)
            {
                return;
            }

            WriteCommand("#STOP");
            port.DataReceived -= Port_DataReceived;
            port.Close();
            OnPropertyChanged(nameof(IsPortOpen));
            port = null;
        }

        public void WriteCommand(string command)
        {
            if (port == null || !IsPortOpen)
            {
                return;
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => SendedCommands.Add(command));
            
            port.WriteLine(command);
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = port.ReadLine();
            RecivedData.Add(data);
        }
    }
}
