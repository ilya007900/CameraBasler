using CameraBasler.Model;

namespace CameraBasler.ViewModel
{
    public class PupilReactionViewModel : ViewModel
    {
        private readonly PupilReactionModel model;

        private bool inProgress;
        private bool showGraphics;

        public byte StartingBrightLevel
        {
            get => model.StartingBrightLevel;
            set
            {
                model.StartingBrightLevel = value;
                OnPropertyChanged();
            }
        }

        public double BrightIncreaseCoefficient
        {
            get => model.BrightIncreaseCoefficient;
            set
            {
                model.BrightIncreaseCoefficient = value;
                OnPropertyChanged();
            }
        }

        public int ThresholdBrightnessForLongPulses
        {
            get => model.ThresholdBrightnessForLongPulses;
            set
            {
                model.ThresholdBrightnessForLongPulses = value;
                OnPropertyChanged();
            }
        }

        public int AveragePupilReactionTime
        {
            get => model.AveragePupilReactionTime;
            set
            {
                model.AveragePupilReactionTime = value;
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

        public PupilReactionViewModel()
        {
            model = new PupilReactionModel();
        }
    }
}
