using CameraBasler.Commands;
using CameraBasler.Events;
using CameraBasler.Model;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CameraBasler.ViewModel
{
    public class CameraViewModel : ViewModel
    {
        private CameraModel model;
        private bool isCameraOpen = false;
        private bool isGrabbing = false;
        private string name;
        private SolidColorBrush cameraStateButtonColor;
        private string cameraStateButtonContent;

        private ICommand changeCameraStateCommand;
        private ICommand startVideoCommand;
        private ICommand stopVideoCommand;

        private ExposureViewModel exposureViewModel;
        private GainViewModel gainViewModel;

        public CameraModel Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public ExposureViewModel ExposureViewModel
        {
            get => exposureViewModel;
            private set
            {
                exposureViewModel = value;
                OnPropertyChanged();
            }
        }

        public GainViewModel GainViewModel
        {
            get => gainViewModel;
            private set
            {
                gainViewModel = value;
                OnPropertyChanged();
            }
        } 

        public bool IsCameraOpen
        {
            get => isCameraOpen;
            set
            {
                isCameraOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsGrabbing
        {
            get => isGrabbing;
            set
            {
                isGrabbing = value;
                OnPropertyChanged();
            }
        }

        public string Name 
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush CameraStateButtonColor
        {
            get => cameraStateButtonColor ?? Brushes.Red;
            set
            {
                cameraStateButtonColor = value;
                OnPropertyChanged();
            }
        }

        public string CameraStateButtonContent
        {
            get => cameraStateButtonContent ?? "Off";
            set
            {
                cameraStateButtonContent = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<CameraBitmapEventArgs> OnImageRecived;

        public ICommand ChangeCameraStateCommand => changeCameraStateCommand ??
            (changeCameraStateCommand = new RelayCommand(ChangeCameraStateAction));

        public ICommand StartVideoCommand => startVideoCommand ??
            (startVideoCommand = new RelayCommand(obj => StartGrab()));

        public ICommand StopVideoCommand => stopVideoCommand ??
            (stopVideoCommand = new RelayCommand(obj => StopGrab()));

        public void OpenCamera()
        {
            Model = new CameraModel();
            Model.Open();
            ExposureViewModel = new ExposureViewModel(Model);
            GainViewModel = new GainViewModel(Model);
            IsCameraOpen = true;
            Name = Model.Name;
        }

        public void CloseCamera()
        {
            Model.Close();
            Model = null;
            ExposureViewModel = null;
            GainViewModel = null;
            IsCameraOpen = false;
            Name = null;
        }

        public void StartGrab()
        {
            Model.ImageGrabbed += Model_ImageGrabbed;
            IsGrabbing = true;
            Model.Start();
        }

        public void StopGrab()
        {
            Model.ImageGrabbed -= Model_ImageGrabbed;
            IsGrabbing = false;
            Model.Stop();
        }

        private void Model_ImageGrabbed(object sender, CameraBitmapEventArgs e)
        {
            OnImageRecived?.Invoke(sender, e);
        }

        private void ChangeCameraStateAction(object obj)
        {
            if (IsCameraOpen)
            {
                CloseCamera();
                CameraStateButtonColor = Brushes.Red;
                CameraStateButtonContent = "Off";
            }
            else
            {
                try
                {
                    OpenCamera();
                    CameraStateButtonColor = Brushes.Green;
                    CameraStateButtonContent = "On";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
