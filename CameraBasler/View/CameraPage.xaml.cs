using Basler.Pylon;
using CameraBasler.Model;
using CameraBasler.ViewModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CameraBasler.View
{
    /// <summary>
    /// Interaction logic for CameraPage.xaml
    /// </summary>
    public partial class CameraPage : Page
    {
        private readonly CameraViewModel viewModel;

        public CameraPage()
        {
            viewModel = new CameraViewModel(new CameraModel());
            DataContext = viewModel;
            viewModel.model.Open();

            InitializeComponent();

            AutoExposure.IsChecked = viewModel.model.ExposureAuto;
            ExposureValue.IsEnabled = !AutoExposure.IsChecked.Value;
            ExposureSlider.IsEnabled = !AutoExposure.IsChecked.Value;

            ExposureSlider.Value = viewModel.model.ExposureTime;
            ExposureSlider.Minimum = viewModel.model.ExposureTimeMin;
            ExposureSlider.Maximum = viewModel.model.ExposureTimeMax;

            ExposureValue.Text = viewModel.model.ExposureTime.ToString();

            AutoGain.IsChecked = viewModel.model.GainAuto;
            GainSlider.IsEnabled = !AutoGain.IsChecked.Value;
            GainValue.IsEnabled = !AutoGain.IsChecked.Value;

            GainSlider.Value = viewModel.model.Gain;
            GainSlider.Minimum = viewModel.model.GainMin;
            GainSlider.Maximum = viewModel.model.GainMax;

            GainValue.Text = viewModel.model.Gain.ToString();
        }

        private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.ImageGrabbed += Model_ImageGrabbed;
            viewModel.model.Start();
        }

        private void Model_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CameraImage.Source = GetImage(e.GrabResult);
            });
        }

        private void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.ImageGrabbed -= Model_ImageGrabbed;
            viewModel.model.Stop();
        }

        private static BitmapImage GetImage(IGrabResult grabResult)
        {
            var converter = new PixelDataConverter();
            var bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
            // Lock the bits of the bitmap.
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            // Place the pointer to the buffer of the bitmap.
            converter.OutputPixelFormat = PixelType.BGRA8packed;
            var ptrBmp = bmpData.Scan0;
            converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
            bitmap.UnlockBits(bmpData);

            var imageConverter = new ImageConverter();
            var bytes = (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
            var img = LoadImage(bytes);
            return img;
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void AutoExposure_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.ExposureAuto = true;
            ExposureSlider.IsEnabled = false;
            ExposureValue.IsEnabled = false;
        }

        private void AutoExposure_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.ExposureAuto = false;
            ExposureSlider.IsEnabled = true;
            ExposureSlider.Value = viewModel.model.ExposureTime;
            ExposureValue.IsEnabled = true;
            ExposureValue.Text = viewModel.model.ExposureTime.ToString();
        }

        private void ExposureSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.model.ExposureTime = e.NewValue;
            ExposureValue.Text = ((int)e.NewValue).ToString();
        }

        private void ExposureValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(x => char.IsDigit(x));
        }

        private void AutoGain_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.GainAuto = true;
            GainSlider.IsEnabled = false;
            GainValue.IsEnabled = false;
        }

        private void AutoGain_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.model.GainAuto = false;
            GainSlider.IsEnabled = true;
            GainSlider.Value = viewModel.model.Gain;
            GainValue.IsEnabled = true;
            GainValue.Text = viewModel.model.Gain.ToString();
        }

        private void GainValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(x => char.IsDigit(x));
        }

        private void GainSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.model.Gain = e.NewValue;
            GainValue.Text = e.NewValue.ToString();
        }
    }
}
