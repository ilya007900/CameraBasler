namespace CameraBasler.Model
{
    public class PupilReactionModel:Model
    {
        private byte startingBrightLevel;
        private double brightIncreaseCoefficient;
        private int thresholdBrightnessForLongPulses;
        private int averagePupilReactionTime;

        public byte StartingBrightLevel
        {
            get => startingBrightLevel;
            set
            {
                startingBrightLevel = value;
                OnPropertyChanged();
            }
        }

        public double BrightIncreaseCoefficient 
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
    }
}
