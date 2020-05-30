using CameraBasler.Commands;
using CameraBasler.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CameraBasler.ViewModel
{
    public class ArduinoViewModel : ViewModel
    {
        private ArduinoModel model;

        private string selectedPort;
        private string commandText;

        private ICommand connectCommand;
        private ICommand disconnectCommand;
        private ICommand refreshPortsCommand;
        private ICommand executeCommand;

        public ArduinoModel Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPort
        {
            get => selectedPort;
            set
            {
                selectedPort = value;
                OnPropertyChanged();
            }
        }

        public string CommandText
        {
            get => commandText;
            set
            {
                commandText = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> AvailablePorts => ArduinoModel.AvailablePorts;

        public ICommand ConnectCommand => connectCommand ??
            (connectCommand = new RelayCommand(obj => Connect()));

        public ICommand DisconnectCommand => disconnectCommand ??
            (disconnectCommand = new RelayCommand(obj => Disconnect()));

        public ICommand RefreshPortsCommand => refreshPortsCommand ??
            (refreshPortsCommand = new RelayCommand(obj => RefreshPorts()));

        public ICommand ExecuteCommand => executeCommand ??
            (executeCommand = new RelayCommand(obj => model.WriteCommand(CommandText)));

        public void RefreshPorts()
        {
            OnPropertyChanged(nameof(AvailablePorts));
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(SelectedPort))
            {
                MessageBox.Show("Port not selected");
                return;
            }

            Model = new ArduinoModel(SelectedPort);
        }

        public void Disconnect()
        {
            Model.Disconnect();
        }
    }
}
