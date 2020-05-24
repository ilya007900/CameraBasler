using System;

namespace CameraBasler.Events
{
    public class CommandEventArgs : EventArgs
    {
        public string Command { get; }

        public CommandEventArgs(string command)
        {
            Command = command;
        }
    }
}
