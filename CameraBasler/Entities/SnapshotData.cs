using System;
using System.Drawing;

namespace CameraBasler.Entities
{
    public class SnapshotData
    {
        public Bitmap Bitmap { get; set; }

        public double ExposureTime { get; set; }

        public double Gain { get; set; }

        public string PixelFormat { get; set; }

        public DateTime DateTime { get; set; }
    }
}
