using CameraBasler.Model;
using System.Collections.Generic;
using System.IO.Ports;

namespace CameraBasler.ViewModel
{
    public class ArduinoViewModel : ViewModel
    {
        private readonly ArduinoModel model;

        private string selectedPort;
        private string command;
        private string recivedData;
        private string errorMessage;

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

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> AvailablePorts => model.AvailablePorts;

        public bool IsPortOpen => model.IsOpen;

        public ArduinoViewModel()
        {
            model = new ArduinoModel();
        }

        public void RefreshPorts()
        {
            OnPropertyChanged(nameof(AvailablePorts));
        }

        public void Connect()
        {
            model.Connect(SelectedPort);
            model.OnDataRecived += Model_OnDataRecived;
        }

        public void Disconnect()
        {
            model.OnDataRecived -= Model_OnDataRecived;
            model.Disconnect();
        }

        public void WriteCommand()
        {
            model.WriteCommand(Command);
        }

        public bool SendInitCommand()
        {
            var result = model.SendInitCommand();
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendInitCommand)}";
            }

            return result;
        }

        public bool SendIRLEDSwitchCommand(bool flag)
        {
            var result = model.SendIRLEDSwitchCommand(flag);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendIRLEDSwitchCommand)}";
            }

            return result;
        }

        public bool SendBlueLedSwitchCommand(bool flag)
        {
            var result = model.SendBlueLedSwitchCommand(flag);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendBlueLedSwitchCommand)}";
            }

            return result;
        }

        public bool SendPWMSetCommand(byte code)
        {
            var result = model.SendPWMSetCommand(code);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendPWMSetCommand)}";
            }

            return result;
        }

        public bool SendLEDNSwitchCommad(byte number, bool flag)
        {
            var result = model.SendLEDNSwitchCommad(number, flag);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendLEDNSwitchCommad)}";
            }

            return result;
        }

        public bool SendEnableCommand(bool flag)
        {
            var result = model.SendEnableCommand(flag);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendEnableCommand)}";
            }

            return result;
        }

        public bool SendEnableCommand(short number)
        {
            var result = model.SendEnableCommand(number);
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendEnableCommand)}";
            }

            return result;
        }

        public bool SendStopCommand()
        {
            var result = model.SendStopCommand();
            if (!result)
            {
                ErrorMessage = $"Error in {nameof(model.SendStopCommand)}";
            }

            return result;
        }

        private void Model_OnDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            RecivedData = model.ReadPortData();
        }
    }
}
