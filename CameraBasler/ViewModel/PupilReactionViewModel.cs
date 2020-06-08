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

        private readonly List<PupilReactionSnapshotData> savedImages = new List<PupilReactionSnapshotData>();

        public PupilReactionModel Model { get; } = new PupilReactionModel();

        private ArduinoModel ArduinoModel { get; }

        private CameraModel CameraModel { get; }

        private bool inProgress;
        private bool isAutoMode;
        private string state;
        private bool isTabSelected;

        private ICommand startCommand;
        private ICommand stopCommand;
        private ICommand increaseBrightCommand;

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

        public ICommand StartCommand => startCommand ?? 
            (startCommand = new RelayCommand(obj => Start()));

        public ICommand StopCommand => stopCommand ?? 
            (stopCommand = new RelayCommand(obj => Stop()));

        public ICommand IncreaseBrightCommand => increaseBrightCommand ??
            (increaseBrightCommand = new RelayCommand(obj => IncreaseBright()));

        public PupilReactionViewModel(ArduinoModel arduinoModel, CameraModel cameraModel)
        {
            ArduinoModel = arduinoModel;
            CameraModel = cameraModel;
        }

        public void Start()
        {
            InProgress = true;
            State = "InProgress";
            CameraModel.Start();
            Model.CurrentBright = Model.StartingBrightLevel;

            ArduinoModel.WriteCommand("#LEDAON");
            var brightCommand = $"#PWMB{Model.StartingBrightLevel}";
            ArduinoModel.WriteCommand(brightCommand);

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
            ArduinoModel.WriteCommand("#LEDAOFF");
            ArduinoModel.WriteCommand("#LEDBOFF");
            InProgress = false;
            CameraModel.Stop();
            State = "Finished";
            SaveImages();
        }

        public void Snapshot()
        {
            var bytes = CameraModel.Snapshot();
            var snapshotData = new PupilReactionSnapshotData
            {
                Bytes = bytes,
                DateTime = DateTime.Now,
                ExposureTime = CameraModel.ExposureTime,
                Gain = CameraModel.Gain,
                PixelFormat = CameraModel.PixelFormat,
                PWM = Model.CurrentBright
            };

            savedImages.Add(snapshotData);
        }

        public void IncreaseBright()
        {
            var pwm = Model.CurrentBright + Model.BrightIncreaseCoefficient;
            if (pwm < byte.MinValue)
            {
                return;
            }

            if (pwm > byte.MaxValue)
            {
                pwm = byte.MaxValue;
            }

            var asUshort = (ushort)pwm;
            Model.CurrentBright = asUshort;
            Snapshot();
            ArduinoModel.WriteCommand("#PWMB" + asUshort.ToString());
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

            var jObject = new
            {
                StartBrightLevel = Model.StartingBrightLevel,
                LastBrightLevel = Model.CurrentBright,
                Snapshots = snapshotsData,
            };

            var json = JsonConvert.SerializeObject(jObject);
            var jsonFileName = Path.Combine(WorkingDirectory, $"{name}{index}.json");
            File.WriteAllText(jsonFileName, json);
        }
    }
}
