using CameraBasler.Events;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CameraBasler.ViewModel
{
    public class MainPageViewModel : ViewModel
    {
        private BitmapImage image;
        private CameraViewModel cameraViewModel;

        private PupilReactionViewModel pupilReactionViewModel;
        public BrightnessDistributionViewModel brightnessDistributionViewModel;

        public bool isPupilReactionViewModelSelected;
        public bool isBrightnessDistributionViewModelSelected;

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

        public CameraViewModel CameraViewModel
        {
            get => cameraViewModel;
            set
            {
                cameraViewModel = value;
                OnPropertyChanged();
            }
        }

        public PupilReactionViewModel PupilReactionViewModel 
        {
            get => pupilReactionViewModel;
            private set
            {
                pupilReactionViewModel = value;
                OnPropertyChanged();
            }
        }

        public BrightnessDistributionViewModel BrightnessDistributionViewModel
        {
            get => brightnessDistributionViewModel;
            set
            {
                brightnessDistributionViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsPupilReactionViewModelSelected 
        { 
            get => isPupilReactionViewModelSelected;
            set
            {
                isPupilReactionViewModelSelected = value;
                OnPropertyChanged();

                if (PupilReactionViewModel == null)
                {
                    PupilReactionViewModel = new PupilReactionViewModel(ArduinoViewModel.Model, CameraViewModel.Model);
                }

                PupilReactionViewModel.IsTabSelected = value;
            }
        }

        public bool IsBrightnessDistributionViewModelSelected
        {
            get => isBrightnessDistributionViewModelSelected;
            set
            {
                isBrightnessDistributionViewModelSelected = value;
                OnPropertyChanged();

                if (BrightnessDistributionViewModel == null)
                {
                    BrightnessDistributionViewModel = new BrightnessDistributionViewModel(ArduinoViewModel.Model, CameraViewModel.Model);
                }
            }
        }

        public MainPageViewModel()
        {
            CameraViewModel = new CameraViewModel();
            CameraViewModel.OnImageRecived += CameraViewModel_OnImageRecived;
            ArduinoViewModel = new ArduinoViewModel();
        }

        private void CameraViewModel_OnImageRecived(object sender, CameraBitmapEventArgs e)
        {
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
