using CameraBasler.Commands;
using CameraBasler.Entities;
using CameraBasler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CameraBasler.ViewModel
{
    public class PupilReactionViewModel : ViewModel
    {
        private const string WorkingDirectory = "C://CameraBaslerNET";

        private PupilReactionModel model;
        private readonly ArduinoViewModel arduinoViewModel;
        private readonly CameraViewModel cameraViewModel;
        private readonly List<SnapshotData> savedImages = new List<SnapshotData>();

        private bool inProgress;
        private bool showGraphics;
        private string state;
        private byte currentBright;

        private ICommand startCommand;
        private ICommand stopCommand;
        private ICommand snapshotCommand;
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

        public string State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public byte CurrentBright
        {
            get => currentBright;
            set
            {
                currentBright = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => startCommand ?? 
            (startCommand = new RelayCommand(obj => Start()));

        public ICommand StopCommand => stopCommand ?? 
            (stopCommand = new RelayCommand(obj => Stop()));

        public ICommand SnapshotCommand => snapshotCommand ?? 
            (snapshotCommand = new RelayCommand(obj => Snapshot()));

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
            CurrentBright = model.StartingBrightLevel;
            arduinoViewModel.Model.WriteCommand("#LEDAON");
            arduinoViewModel.Model.WriteCommand("#LEDBON");
        }

        public void Stop()
        {
            arduinoViewModel.Model.WriteCommand("#LEDAOFF");
            arduinoViewModel.Model.WriteCommand("#LEDBOFF");
            cameraViewModel.StopGrab();
            InProgress = false;
            State = "Finished";
            SaveImages();
        }

        public void Snapshot()
        {
            var bytes = cameraViewModel.Model.Snapshot();
            var snapshotData = new SnapshotData
            {
                Bytes = bytes,
                DateTime = DateTime.Now,
                ExposureTime = cameraViewModel.Model.ExposureTime,
                Gain = cameraViewModel.Model.Gain,
                PixelFormat = cameraViewModel.Model.PixelFormat
            };

            savedImages.Add(snapshotData);
        }

        public void IncreaseBright()
        {
            var pwm = CurrentBright * model.BrightIncreaseCoefficient;
            if (pwm >= 0 && pwm <= 255) {
                var asByte = (byte)pwm;
                CurrentBright = asByte;
                Snapshot();
                arduinoViewModel.Model.WriteCommand("#PWMB" + asByte.ToString());
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
                x.PixelFormat
            }).ToArray();

            var json = JsonConvert.SerializeObject(snapshotsData);
            var jsonFileName = Path.Combine(WorkingDirectory, $"{name}{index}.json");
            File.WriteAllText(jsonFileName, json);
        }
    }
}
