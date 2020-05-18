using CameraBasler.Events;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CameraBasler.ViewModel
{
    public class MainPageViewModel : ViewModel
    {
        private BitmapImage image;

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

        public CameraViewModel CameraViewModel { get; }

        public PupilReactionViewModel PupilReactionViewModel { get; }

        public BrightnessDistributionViewModel BrightnessDistributionViewModel { get; }

        public MainPageViewModel()
        {
            CameraViewModel = new CameraViewModel();
            ArduinoViewModel = new ArduinoViewModel();
            PupilReactionViewModel = new PupilReactionViewModel();
            BrightnessDistributionViewModel = new BrightnessDistributionViewModel();
        }

        public void StartVideo()
        {
            CameraViewModel.OnImageRecived += CameraViewModel_OnImageRecived;
            CameraViewModel.StartGrab();
        }

        public void StopVideo()
        {
            CameraViewModel.OnImageRecived -= CameraViewModel_OnImageRecived;
            CameraViewModel.StopGrab();
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
