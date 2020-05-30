using System;

namespace CameraBasler.Entities
{
    public class PupilReactionSnapshotData
    {
        public byte[] Bytes { get; set; }

        public double ExposureTime { get; set; }

        public double Gain { get; set; }

        public string PixelFormat { get; set; }

        public ushort PWM { get; set; }

        public DateTime DateTime { get; set; }
    }
}
