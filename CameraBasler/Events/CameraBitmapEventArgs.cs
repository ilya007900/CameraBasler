using System;
using System.Drawing;

namespace CameraBasler.Events
{
    public class CameraBitmapEventArgs : EventArgs
    {
        public Bitmap Bitmap { get; }

        public CameraBitmapEventArgs(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}
