namespace CameraBasler.Model
{
    public class PupilReactionModel
    {
        public byte StartingBrightLevel { get; set; }

        public double BrightIncreaseCoefficient { get; set; }

        public int ThresholdBrightnessForLongPulses { get; set; }

        public int AveragePupilReactionTime { get; set; }
    }
}
