using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace CameraBasler.Model
{
    public class ArduinoModel
    {
        private const string OnCommand = "ON";
        private const string OffCommand = "OFF";

        private const string InitCommand = "Basler_arduino_init";
        private const string IRLEDSwitchCommand = "LEDA";
        private const string BlueLedSwitchCommand = "LEDB";
        private const string PWMSetCommand = "PWMB{0}";
        private const string LEDNSwitchCommad = "LED{0}";
        private const string EnableCommand = "ENBL";
        private const string StopCommand = "STOP";

        private const string ConnectSuccessCommand = "Basler_helmet_arduin";
        private const string DoneCommand = "Done!";

        private SerialPort port;

        public bool IsOpen => port == null ? false : port.IsOpen;

        public IEnumerable<string> AvailablePorts => SerialPort.GetPortNames();

        public event EventHandler<SerialDataReceivedEventArgs> OnDataRecived;

        public void Connect(string portName)
        {
            if (string.IsNullOrEmpty(portName))
            {
                return;
            }

            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += Port_DataReceived;
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
            port.DataReceived -= Port_DataReceived;
            port.Close();
            port = null;
        }

        public void WriteCommand(string command)
        {
            if (port == null)
            {
                return;
            }

            port.Write(command);
        }

        public string ReadPortData()
        {
            return port.ReadLine();
        }

        public bool SendInitCommand()
        {
            if (!SendCommand(InitCommand))
            {
                return false;
            }

            var answer = port.ReadLine();
            return answer == ConnectSuccessCommand;
        }

        public bool SendIRLEDSwitchCommand(bool flag)
        {
            var command = GetCommandWithFlag(IRLEDSwitchCommand, flag);
            return SendCommand(command);
        }

        public bool SendBlueLedSwitchCommand(bool flag)
        {
            var command =  GetCommandWithFlag(BlueLedSwitchCommand, flag);
            return SendCommand(command);
        }

        public bool SendPWMSetCommand(byte code)
        {
            var command = string.Format(PWMSetCommand, code);
            return SendCommand(command);
        }

        public bool SendLEDNSwitchCommad(byte number, bool flag)
        {
            var command = string.Format(LEDNSwitchCommad, number);
            return SendCommand(GetCommandWithFlag(command, flag));
        }

        public bool SendEnableCommand(bool flag)
        {
            var command = GetCommandWithFlag(EnableCommand, flag);
            return SendCommand(command);
        }

        public bool SendEnableCommand(short number)
        {
            var command = number.ToString();
            if (!SendCommand(command))
            {
                return false;
            }

            var answer = port.ReadLine();
            return answer == DoneCommand;
        }

        public bool SendStopCommand()
        {
            return SendCommand(StopCommand);
        }

        private bool SendCommand(string command)
        {
            if (port == null || !port.IsOpen)
            {
                return false;
            }

            port.WriteLine(command);
            return true;
        }

        private static string GetCommandWithFlag(string command, bool flag)
        {
            return command + (flag ? OnCommand : OffCommand);
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnDataRecived?.Invoke(sender, e);
        }
    }
}
