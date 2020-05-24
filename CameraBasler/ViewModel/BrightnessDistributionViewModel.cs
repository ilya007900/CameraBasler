using System.Collections.ObjectModel;

namespace CameraBasler.ViewModel
{
    public class BrightnessDistributionViewModel : ViewModel
    {
        private ObservableCollection<DiodViewModel> DiodViewModels = new ObservableCollection<DiodViewModel>
        {
            new DiodViewModel
            {
                Id = 1,
            },
            new DiodViewModel
            {
                Id = 2,
            },
            new DiodViewModel
            {
                Id = 3,
            },
            new DiodViewModel
            {
                Id = 4,
            },
            new DiodViewModel
            {
                Id = 5,
            },
            new DiodViewModel
            {
                Id = 6,
            },
            new DiodViewModel
            {
                Id = 7,
            }
        };

        private bool tauTuning;
        private bool inProgress;

        public ObservableCollection<DiodViewModel> Diods
        {
            get => DiodViewModels;
            set
            {
                DiodViewModels = value;
                OnPropertyChanged();
            }
        }

        public bool TauTuning
        {
            get => tauTuning;
            set
            {
                tauTuning = value;
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
