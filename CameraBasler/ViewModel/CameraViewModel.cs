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
        private SolidColorBrush cameraStateButtonColor;
        private string cameraStateButtonContent;

        private ICommand changeCameraStateCommand;
        private ICommand startVideoCommand;
        private ICommand stopVideoCommand;

        public CameraModel Model
        {
            get => model;
            set
            {
                model = value;
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
            var cameraModel = new CameraModel();
            cameraModel.Open();
            Model = cameraModel;
            Model.ImageGrabbed += Model_ImageGrabbed;
        }

        public void CloseCamera()
        {
            Model.ImageGrabbed -= Model_ImageGrabbed;
            Model.Close();
            Model = null;
        }

        public void StartGrab()
        {
            Model.Start();
        }

        public void StopGrab()
        {
            Model.Stop();
        }

        private void Model_ImageGrabbed(object sender, CameraBitmapEventArgs e)
        {
            OnImageRecived?.Invoke(sender, e);
        }

        private void ChangeCameraStateAction(object obj)
        {
            if (Model == null)
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
            else
            {
                CloseCamera();
                CameraStateButtonColor = Brushes.Red;
                CameraStateButtonContent = "Off";
            }
        }
    }
}
