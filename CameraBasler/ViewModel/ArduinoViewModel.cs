using CameraBasler.Commands;
using CameraBasler.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace CameraBasler.ViewModel
{
    public class ArduinoViewModel : ViewModel
    {
        private readonly ArduinoModel model;
        private ObservableCollection<string> sendedCommands;

        private string selectedPort;
        private string command;
        private string recivedData;
        private string errorMessage;
        private bool isConnected = false;

        private ICommand connectCommand;
        private ICommand disconnectCommand;
        private ICommand refreshPortsCommand;
        private ICommand executeCommand;

        public ObservableCollection<string> SendedCommands
        {
            get => sendedCommands;
            set
            {
                sendedCommands = value;
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

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand => connectCommand ??
            (connectCommand = new RelayCommand(obj => Connect()));

        public ICommand DisconnectCommand => disconnectCommand ??
            (disconnectCommand = new RelayCommand(obj => Disconnect()));

        public ICommand RefreshPortsCommand => refreshPortsCommand ??
            (refreshPortsCommand = new RelayCommand(obj => RefreshPorts()));

        public ICommand ExecuteCommand => executeCommand ??
            (executeCommand = new RelayCommand(obj => WriteCommand()));

        public ArduinoViewModel()
        {
            model = new ArduinoModel();
            model.OnCommandSended += Model_OnCommandSended;
            SendedCommands = new ObservableCollection<string>();
        }

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

            model.Connect(SelectedPort);
            IsConnected = model.IsOpen;
            model.OnDataRecived += Model_OnDataRecived;
        }

        public void Disconnect()
        {
            model.OnDataRecived -= Model_OnDataRecived;
            model.Disconnect();
            IsConnected = model.IsOpen;
        }

        public void WriteCommand()
        {
            model.WriteCommand(Command);
        }

        public void WriteCommand(string command)
        {
            model.WriteCommand(command);
        }

        private void Model_OnCommandSended(object sender, Events.CommandEventArgs e)
        {
            SendedCommands.Add(e.Command);
        }

        private void Model_OnDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            RecivedData = model.ReadPortData();
        }
    }
}
