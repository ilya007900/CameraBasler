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
                IsUsing = true,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 0
            },
            new DiodViewModel
            {
                Id = 2,
                IsUsing = true,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 1
            },
            new DiodViewModel
            {
                Id = 3,
                IsUsing = true,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 2
            },
            new DiodViewModel
            {
                Id = 4,
                IsUsing = false,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 4
            },
            new DiodViewModel
            {
                Id = 5,
                IsUsing = false,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 8
            },
            new DiodViewModel
            {
                Id = 6,
                IsUsing = true,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 16
            },
            new DiodViewModel
            {
                Id = 7,
                IsUsing = true,
                MaxEnergy = 630,
                Tau = 40,
                Km1 = 5,
                Km2 = 25,
                Step = 32
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
