using CameraBasler.Commands;
using CameraBasler.Interfaces;
using CameraBasler.Model;
using CameraBasler.Services;
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
        private readonly IDialogService dialogService = new DefaultDialogService();
        private readonly IFileService fileService = new JsonFileService();

        private ObservableCollection<DiodViewModel> diods = new ObservableCollection<DiodViewModel>
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
        private DiodViewModel selectedDiod;

        private ArduinoModel ArduinoModel { get; }

        private CameraModel CameraModel { get; }

        private readonly List<object> SavedSnapshots = new List<object>();

        private bool tauTuning;
        private bool inProgress;
        private bool isTabSelected;

        private ICommand startCommand;
        private ICommand loadDiodsCommand;
        private ICommand saveDiodsCommand;

        public ObservableCollection<DiodViewModel> Diods
        {
            get => diods;
            set
            {
                diods = value;
                OnPropertyChanged();
            }
        }

        public DiodViewModel SelectedDiod
        {
            get => selectedDiod;
            set
            {
                selectedDiod = value;
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

        public bool IsTabSelected
        {
            get => isTabSelected;
            set
            {
                isTabSelected = value;
                OnPropertyChanged();
                if (isTabSelected)
                {
                    ArduinoModel.WriteCommand("#LEDAON");
                }
                else
                {
                    ArduinoModel.WriteCommand("#LEDAOFF");
                }
            }
        }

        public ICommand StartCommand => startCommand ?? (startCommand = new RelayCommand(obj =>
        {
            var thread = new Thread(Start);
            thread.Start();
        }));

        public ICommand LoadDiodsCommand => loadDiodsCommand ?? (loadDiodsCommand = new RelayCommand(obj =>
        {
            try
            {
                if (dialogService.OpenFileDialog("Text file (*.json)|*.json") == true)
                {
                    var fileDiods = fileService.Open(dialogService.FilePath);
                    Diods = new ObservableCollection<DiodViewModel>(fileDiods.Select(x => new DiodViewModel
                    {
                        DiodModel = x
                    }).ToList());

                    dialogService.ShowMessage("Файл открыт");
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message);
            }
        }));

        public ICommand SaveDiodsCommand => saveDiodsCommand ?? (saveDiodsCommand = new RelayCommand(obj =>
        {
            try
            {
                if (dialogService.SaveFileDialog("Text file (*.json)|*.json") == true)
                {
                    fileService.Save(dialogService.FilePath, Diods.Select(x => x.DiodModel).ToList());
                    dialogService.ShowMessage("Файл сохранен");
                }
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage(ex.Message);
            }
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

                ArduinoModel.WriteCommand("#LEDAOFF");
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
