namespace CameraBasler.ViewModel
{
    public class PupilReactionViewModel : ViewModel
    {
        private byte startingBrightLevel = 32;
        private double brightIncreaseCoefficient = 1.5;
        private int thresholdBrightnessForLongPulses = 132;
        private int averagePupilReactionTime = 150;

        private bool inProgress;
        private bool showGraphics;

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

        public bool ShowGraphics
        {
            get => showGraphics;
            set
            {
                showGraphics = value;
                OnPropertyChanged();
            }
        }

        public bool InProgress
        {
            get => inProgress;
            set
            {
                inProgress = value;
                OnPropertyChanged();
            }
        }
    }
}
