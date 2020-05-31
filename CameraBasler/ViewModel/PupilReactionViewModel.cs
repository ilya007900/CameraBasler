using CameraBasler.Commands;
using CameraBasler.Entities;
using CameraBasler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace CameraBasler.ViewModel
{
    public class PupilReactionViewModel : ViewModel
    {
        private const string WorkingDirectory = "C://CameraBaslerNET";

        private PupilReactionModel model;
        private readonly ArduinoViewModel arduinoViewModel;
        private readonly CameraViewModel cameraViewModel;
        private readonly List<PupilReactionSnapshotData> savedImages = new List<PupilReactionSnapshotData>();

        private bool inProgress;
        private bool isAutoMode;
        private string state;

        private ICommand startCommand;
        private ICommand stopCommand;
        private ICommand increaseBrightCommand;

        public PupilReactionModel Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public bool IsAutoMode
        {
            get => isAutoMode;
            set
            {
                isAutoMode = value;
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

        public string State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => startCommand ?? 
            (startCommand = new RelayCommand(obj => Start()));

        public ICommand StopCommand => stopCommand ?? 
            (stopCommand = new RelayCommand(obj => Stop()));

        public ICommand IncreaseBrightCommand => increaseBrightCommand ??
            (increaseBrightCommand = new RelayCommand(obj => IncreaseBright()));

        public PupilReactionViewModel(ArduinoViewModel arduinoViewModel, CameraViewModel cameraViewModel)
        {
            model = new PupilReactionModel();
            this.arduinoViewModel = arduinoViewModel;
            this.cameraViewModel = cameraViewModel;
        }

        public void Start()
        {
            InProgress = true;
            State = "InProgress";
            cameraViewModel.StartGrab();
            model.CurrentBright = model.StartingBrightLevel;

            arduinoViewModel.Model.WriteCommand("#LEDAON");
            var brightCommand = $"#PWMB{model.StartingBrightLevel}";
            arduinoViewModel.Model.WriteCommand(brightCommand);

            if (IsAutoMode)
            {
                var thread = new Thread(() =>
                {
                    while (InProgress)
                    {
                        Thread.Sleep(2000);
                        if (InProgress)
                        {
                            IncreaseBright();
                        }
                        else
                        {
                            break;
                        }
                    }
                });
                thread.Start();
            }
        }

        public void Stop()
        {
            arduinoViewModel.Model.WriteCommand("#LEDAOFF");
            arduinoViewModel.Model.WriteCommand("#LEDBOFF");
            InProgress = false;
            cameraViewModel.StopGrab();
            State = "Finished";
            SaveImages();
        }

        public void Snapshot()
        {
            var bytes = cameraViewModel.Model.Snapshot();
            var snapshotData = new PupilReactionSnapshotData
            {
                Bytes = bytes,
                DateTime = DateTime.Now,
                ExposureTime = cameraViewModel.Model.ExposureTime,
                Gain = cameraViewModel.Model.Gain,
                PixelFormat = cameraViewModel.Model.PixelFormat,
                PWM = model.CurrentBright
            };

            savedImages.Add(snapshotData);
        }

        public void IncreaseBright()
        {
            var pwm = model.CurrentBright + model.BrightIncreaseCoefficient;
            if (pwm >= byte.MinValue && pwm <= byte.MaxValue) {
                var asUshort = (ushort)pwm;
                model.CurrentBright = asUshort;
                Snapshot();
                arduinoViewModel.Model.WriteCommand("#PWMB" + asUshort.ToString());
            }
        }

        public void SaveImages()
        {
            if (savedImages.Count == 0)
            {
                return;
            }

            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
            }

            var files = Directory.GetFiles(WorkingDirectory).Select(Path.GetFileNameWithoutExtension).ToArray();
            var name = "data";
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
                foreach (var savedImage in savedImages)
                {
                    stream.Write(savedImage.Bytes, 0, savedImage.Bytes.Length);
                }
            }

            var snapshotsData = savedImages.Select(x => new
            {
                x.DateTime,
                x.ExposureTime,
                x.Gain,
                x.PixelFormat,
                x.PWM
            }).ToArray();

            var json = JsonConvert.SerializeObject(snapshotsData);
            var jsonFileName = Path.Combine(WorkingDirectory, $"{name}{index}.json");
            File.WriteAllText(jsonFileName, json);
        }
    }
}
