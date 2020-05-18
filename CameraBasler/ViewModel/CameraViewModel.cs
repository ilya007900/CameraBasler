using CameraBasler.Events;
using CameraBasler.Model;
using System;

namespace CameraBasler.ViewModel
{
    public class CameraViewModel : ViewModel
    {
        private CameraModel model;
        private bool isCameraOpen = false;
        private string name;

        private ExposureViewModel exposureViewModel;
        private GainViewModel gainViewModel;

        private CameraModel Model
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

        public string Name 
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<CameraBitmapEventArgs> OnImageRecived;

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
            Model.Start();
        }

        public void StopGrab()
        {
            Model.ImageGrabbed -= Model_ImageGrabbed;
            Model.Stop();
        }

        private void Model_ImageGrabbed(object sender, CameraBitmapEventArgs e)
        {
            OnImageRecived?.Invoke(sender, e);
        }
    }
}
