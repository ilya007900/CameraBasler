using CameraBasler.Events;
using CameraBasler.Model;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CameraBasler.ViewModel
{
    public class CameraViewModel : ViewModel
    {
        private CameraModel model;
        private bool isCameraOpen = false;
        private string name;

        private ExposureViewModel exposureViewModel;
        private GainViewModel gainViewModel;
        private PupilReactionViewModel pupilReactionViewModel;

        private BitmapImage image;

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

        public PupilReactionViewModel PupilReactionViewModel
        {
            get => pupilReactionViewModel;
            set
            {
                pupilReactionViewModel = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        public ArduinoViewModel ArduinoViewModel { get; }

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

        public CameraViewModel()
        {
            ArduinoViewModel = new ArduinoViewModel();
            PupilReactionViewModel = new PupilReactionViewModel();
        }

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
            //ImageWindow.DisplayImage(0, e.GrabResult);
            var bitmapImage = Convert(e.Bitmap);
            bitmapImage.Freeze();
            Dispatcher.CurrentDispatcher.Invoke(() => Image = bitmapImage);
        }

        private static BitmapImage Convert(System.Drawing.Bitmap bitmap)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }
}
