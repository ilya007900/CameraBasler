using System;

namespace CameraBasler.Entities
{
    public class SnapshotData
    {
        public byte[] Bytes { get; set; }

        public double ExposureTime { get; set; }

        public double Gain { get; set; }

        public string PixelFormat { get; set; }

        public DateTime DateTime { get; set; }
    }
}
