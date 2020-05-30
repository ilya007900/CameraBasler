namespace CameraBasler.Model
{
    public class PupilReactionModel : Model
    {
        private byte startingBrightLevel;
        private byte brightIncreaseCoefficient;
        private int thresholdBrightnessForLongPulses;
        private int averagePupilReactionTime;
        private ushort currentBright;

        public byte StartingBrightLevel
        {
            get => startingBrightLevel;
            set
            {
                startingBrightLevel = value;
                OnPropertyChanged();
            }
        }

        public byte BrightIncreaseCoefficient
        {
            get => brightIncreaseCoefficient;
            set
            {
                brightIncreaseCoefficient = value;
                OnPropertyChanged();
            }
        }

        public int ThresholdBrightnessForLongPulses
        {
            get => thresholdBrightnessForLongPulses;
            set
            {
                thresholdBrightnessForLongPulses = value;
                OnPropertyChanged();
            }
        }

        public int AveragePupilReactionTime
        {
            get => averagePupilReactionTime;
            set
            {
                averagePupilReactionTime = value;
                OnPropertyChanged();
            }
        }

        public ushort CurrentBright
        {
            get => currentBright;
            set
            {
                currentBright = value;
                OnPropertyChanged();
            }
        }
    }
}
