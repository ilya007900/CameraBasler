using System;

namespace CameraBasler.Events
{
    public class OneArgEvent<T> : EventArgs
    {
        public T Arg { get; }

        public OneArgEvent(T arg)
        {
            Arg = arg;
        }
    }
}
