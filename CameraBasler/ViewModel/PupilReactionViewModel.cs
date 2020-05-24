using CameraBasler.Commands;
using CameraBasler.Model;
using System.Windows.Input;

namespace CameraBasler.ViewModel
{
    public class PupilReactionViewModel : ViewModel
    {
        private PupilReactionModel model;
        private readonly ArduinoViewModel arduinoViewModel;
        private readonly CameraViewModel cameraViewModel;

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
            arduinoViewModel.WriteCommand("#LEDAON");
            arduinoViewModel.WriteCommand("#LEDBON");
        }

        public void Stop()
        {
            arduinoViewModel.WriteCommand("#LEDAOFF");
            arduinoViewModel.WriteCommand("#LEDBOFF");
            cameraViewModel.StopGrab();
            InProgress = false;
            State = "Finished";
            cameraViewModel.Model.SaveImages();
        }

        public void Snapshot()
        {
            cameraViewModel.Model.Snapshot();
        }

        public void IncreaseBright()
        {
            var pwm = CurrentBright * model.BrightIncreaseCoefficient;
            if (pwm >= 0 && pwm <= 255) {
                var asByte = (byte)pwm;
                CurrentBright = asByte;
                arduinoViewModel.WriteCommand("#PWMB" + asByte.ToString());
            }
        }
    }
}
