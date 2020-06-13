using CameraBasler.Commands;
using CameraBasler.Entities;
using CameraBasler.Interfaces;
using CameraBasler.Model;
using CameraBasler.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace CameraBasler.ViewModel
{
    public class BrightnessDistributionViewModel : ViewModel
    {
        private const string WorkingDirectory = "C://CameraBaslerNET";

        private readonly ManualResetEvent oSignalEvent = new ManualResetEvent(false);
        private readonly IDialogService dialogService = new DefaultDialogService();
        private readonly IFileService fileService = new JsonFileService();
        private readonly List<BrightnessDistributionSnapshotData> savedSnapshots = new List<BrightnessDistributionSnapshotData>();
        private readonly List<DiodViewModel> defaultDiods = new List<DiodViewModel>
        {
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 1, MaxEnergy = 630 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 2, MaxEnergy = 710 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 3, MaxEnergy = 730 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 4, MaxEnergy = 830 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 5, MaxEnergy = 880 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 6, MaxEnergy = 930 }
            },
            new DiodViewModel
            {
                DiodModel = new DiodModel { Id = 7, MaxEnergy = 980 }
            }
        };

        private ObservableCollection<DiodViewModel> diodViewModels;

        private ArduinoModel ArduinoModel { get; }

        private CameraModel CameraModel { get; }

        private bool tauTuning;
        private bool inProgress;
        private bool isTabSelected;

        private ICommand startCommand;
        private ICommand loadDiodsCommand;
        private ICommand saveDiodsCommand;

        public ObservableCollection<DiodViewModel> Diods => 
            diodViewModels ?? (diodViewModels = new ObservableCollection<DiodViewModel>(defaultDiods));

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
                    var fileDiodsViewModel = fileDiods.Select(x => new DiodViewModel
                    {
                        DiodModel = x
                    }).ToList();

                    Diods.Clear();

                    foreach (var diod in fileDiodsViewModel)
                    {
                        Diods.Add(diod);
                    }

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
            CameraModel.Start();
            CameraModel.ExposureAuto = false;
            ArduinoModel.RecivedData.CollectionChanged += RecivedData_CollectionChanged;

            foreach (var diod in Diods)
            {
                if (!diod.DiodModel.IsUsing)
                {
                    continue;
                }

                diod.Color = Brushes.Red;
                var tau = diod.DiodModel.Tau;
                CameraModel.ExposureTime = tau;
                ArduinoModel.WriteCommand("#ENBLON");
                ArduinoModel.WriteCommand($"{diod.DiodModel.Step}");
                oSignalEvent.WaitOne();
                oSignalEvent.Reset();
                var lastRecivedCommand = ArduinoModel.RecivedData.Last();
                if (!lastRecivedCommand.Equals("Done!\r"))
                {
                    InProgress = false;
                    break;
                }

                ArduinoModel.WriteCommand("#ENBLOFF");
                ArduinoModel.WriteCommand("#LEDAOFF");

                savedSnapshots.Add(TakeSnapshot(diod));

                ArduinoModel.WriteCommand($"#LED{diod.DiodModel.Id}ON");
                Thread.Sleep((int)tau);
                savedSnapshots.Add(TakeSnapshot(diod));

                tau /= diod.DiodModel.Km1;
                CameraModel.ExposureTime = tau;
                Thread.Sleep((int)tau);
                savedSnapshots.Add(TakeSnapshot(diod));

                tau /= diod.DiodModel.Km2;
                CameraModel.ExposureTime = tau;
                Thread.Sleep((int)tau);
                savedSnapshots.Add(TakeSnapshot(diod));

                ArduinoModel.WriteCommand($"#LED{diod.DiodModel.Id}OFF");
                diod.Color = Brushes.White;
            }

            InProgress = false;
            CameraModel.Stop();
            SaveImages();
            ArduinoModel.RecivedData.CollectionChanged -= RecivedData_CollectionChanged;
        }

        private BrightnessDistributionSnapshotData TakeSnapshot(DiodViewModel diod)
        {
            return new BrightnessDistributionSnapshotData
            {
                Image = CameraModel.Snapshot(),
                ExposureTime = CameraModel.ExposureTime,
                PixelFormat = CameraModel.PixelFormat,
                Energy = diod.DiodModel.MaxEnergy,
                DateTime = DateTime.Now
            };
        }

        private void RecivedData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            oSignalEvent.Set();
        }

        public void SaveImages()
        {
            if (savedSnapshots.Count == 0)
            {
                return;
            }

            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
            }

            var files = Directory.GetFiles(WorkingDirectory).Select(Path.GetFileNameWithoutExtension).ToArray();
            var name = "BrightnessDistribution";
            var index = 1;
            foreach (var file in files)
            {
                if (files.Any(x => string.Compare(x, $"{name}{index}") == 0))
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            var imgFileName = Path.Combine(WorkingDirectory, $"{name}{index}.bin");
            using (var stream = File.OpenWrite(imgFileName))
            {
                foreach (var savedImage in savedSnapshots)
                {
                    stream.Write(savedImage.Image, 0, savedImage.Image.Length);
                }
            }

            var snapshotsData = savedSnapshots.Select(x => new
            {
                x.DateTime,
                x.ExposureTime,
                x.Energy,
                x.PixelFormat,
            }).ToArray();

            var json = JsonConvert.SerializeObject(snapshotsData);
            var jsonFileName = Path.Combine(WorkingDirectory, $"{name}{index}.json");
            File.WriteAllText(jsonFileName, json);
        }
    }
}
