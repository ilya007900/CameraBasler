using CameraBasler.Commands;
using CameraBasler.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace CameraBasler.ViewModel
{
    public class BrightnessDistributionViewModel : ViewModel
    {
        private readonly ManualResetEvent oSignalEvent = new ManualResetEvent(false);

        private DiodViewModel selectedDiod;

        private ICommand startCommand;

        public ObservableCollection<DiodViewModel> Diods => new ObservableCollection<DiodViewModel>
        {
            new DiodViewModel
            {
                Id = 1,
                ColorBrush = Brushes.Blue,
                DiodModel = new DiodModel
                {
                    IsUsing = true,
                    MaxEnergy = 630
                }
            },
            new DiodViewModel
            {
                Id = 2,
                ColorBrush = Brushes.PaleGreen,
                DiodModel = new DiodModel
                {
                    MaxEnergy = 710
                }
            },
            new DiodViewModel
            {
                Id = 3,
                DiodModel = new DiodModel
                {
                    IsUsing =true,
                    MaxEnergy = 730
                }
            },
            new DiodViewModel
            {
                Id = 4,
                DiodModel = new DiodModel
                {
                    MaxEnergy = 830
                }
            },
            new DiodViewModel
            {
                Id = 5,
                DiodModel = new DiodModel
                {
                    MaxEnergy = 880
                }
            },
            new DiodViewModel
            {
                Id = 6,
                DiodModel = new DiodModel
                {
                    MaxEnergy = 930
                }
            },
            new DiodViewModel
            {
                Id = 7,
                DiodModel = new DiodModel
                {
                    MaxEnergy = 980
                }
            }
        };

        public DiodViewModel SelectedDiod
        {
            get => selectedDiod;
            set
            {
                selectedDiod = value;
                OnPropertyChanged();
            }
        }

        private ArduinoModel ArduinoModel { get; }

        private CameraModel CameraModel { get; }

        private readonly List<object> SavedSnapshots = new List<object>();

        private bool tauTuning;
        private bool inProgress;

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

        public ICommand StartCommand => startCommand ?? (startCommand = new RelayCommand(obj =>
        {
            var thread = new Thread(Start);
            thread.Start();
        }));

        public BrightnessDistributionViewModel(ArduinoModel arduinoModel, CameraModel cameraModel)
        {
            ArduinoModel = arduinoModel;
            CameraModel = cameraModel;
        }

        public void Start()
        {
            InProgress = true;
            CameraModel.ExposureAuto = false;
            ArduinoModel.SendedCommands.CollectionChanged += SendedCommands_CollectionChanged;

            foreach (var diod in Diods)
            {
                if (!diod.DiodModel.IsUsing)
                {
                    continue;
                }

                ArduinoModel.WriteCommand("#ENBLON");
                ArduinoModel.WriteCommand($"#{diod.DiodModel.Step}");
                oSignalEvent.WaitOne();
                oSignalEvent.Reset();
                ArduinoModel.WriteCommand("#ENBLOFF");
                var lastRecivedCommand = ArduinoModel.RecivedData.Last();
                if (!lastRecivedCommand.Equals("Done!"))
                {
                    InProgress = false;
                    break;
                }

                SavedSnapshots.Add(TakeSnapshot(diod));
                ArduinoModel.WriteCommand("#LEDAON");
                SavedSnapshots.Add(TakeSnapshot(diod));
                CameraModel.ExposureTime = diod.DiodModel.Tau * diod.DiodModel.Km1;
                SavedSnapshots.Add(TakeSnapshot(diod));
                CameraModel.ExposureTime = diod.DiodModel.Tau * diod.DiodModel.Km2;
                SavedSnapshots.Add(TakeSnapshot(diod));
            }

            InProgress = false;
            ArduinoModel.SendedCommands.CollectionChanged -= SendedCommands_CollectionChanged;
        }

        private object TakeSnapshot(DiodViewModel diod)
        {
            return new
            {
                Image = CameraModel.Snapshot(),
                ExposureTime = CameraModel.ExposureTime,
                PixelFormat = CameraModel.PixelFormat,
                Energy = diod.DiodModel.MaxEnergy,
                DateTime = DateTime.Now
            };
        }

        private void SendedCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            oSignalEvent.Set();
        }
    }
}
