using Accord.Video.FFMPEG;
using Basler.Pylon;
using CameraBasler.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CameraBasler.Model
{
    public class CameraModel : Model
    {
        private const string ExposureAutoModeOn = "Continuous";
        private const string ExposureAutoModeOff = "Off";
        private const string GainAutoModeOn = "Continuous";
        private const string GainAutoModeOff = "Off";
        private const string FriendlyNameKey = "FriendlyName";
        private const string WorkingDirectory = "C://CameraBaslerNET";

        private ICamera camera;

        private readonly PixelDataConverter converter = new PixelDataConverter();
        private readonly List<Bitmap> savedImages = new List<Bitmap>();

        private Bitmap lastImage;

        public event EventHandler<CameraBitmapEventArgs> ImageGrabbed;

        #region public properties

        public bool IsOpen => camera?.IsOpen ?? false;

        public string Name => camera?.CameraInfo[FriendlyNameKey] ?? string.Empty;

        public double ExposureTimeMin => camera.Parameters[PLCamera.ExposureTime].GetMinimum();

        public double ExposureTimeMax => camera.Parameters[PLCamera.ExposureTime].GetMaximum();

        public double ExposureTime
        {
            get
            {
                return camera.Parameters[PLCamera.ExposureTime].GetValue();
            }
            set
            {
                if (value <= ExposureTimeMax && value >= ExposureTimeMin && !ExposureAuto)
                {
                    camera.Parameters[PLCamera.ExposureTime].SetValue(value);
                    OnPropertyChanged();
                }
            }
        }

        public bool ExposureAuto
        {
            get
            {
                var currentMode = camera.Parameters[PLCamera.ExposureAuto].GetValue();
                return string.CompareOrdinal(currentMode, ExposureAutoModeOn) == 0;
            }
            set
            {
                if (value)
                {
                    camera.Parameters[PLCamera.ExposureAuto].SetValue(ExposureAutoModeOn);
                }
                else
                {
                    camera.Parameters[PLCamera.ExposureAuto].SetValue(ExposureAutoModeOff);
                }

                OnPropertyChanged();
            }
        }

        public double GainMin => camera.Parameters[PLCamera.Gain].GetMinimum();

        public double GainMax => camera.Parameters[PLCamera.Gain].GetMaximum();

        public double Gain
        {
            get => camera.Parameters[PLCamera.Gain].GetValue();
            set
            {
                if (value >= GainMin && value <= GainMax)
                {
                    camera.Parameters[PLCamera.Gain].SetValue(value);
                    OnPropertyChanged();
                }
            }
        }

        public bool GainAuto
        {
            get
            {
                var currentMode = camera.Parameters[PLCamera.GainAuto].GetValue();
                return string.CompareOrdinal(currentMode, GainAutoModeOn) == 0;
            }
            set
            {
                if (value)
                {
                    camera.Parameters[PLCamera.GainAuto].SetValue(GainAutoModeOn);
                }
                else
                {
                    camera.Parameters[PLCamera.GainAuto].SetValue(GainAutoModeOff);
                }

                OnPropertyChanged();
            }
        }

        public IEnumerable<string> PixelFormats => camera.Parameters[PLCamera.PixelFormat].GetAllValues();

        public string PixelFormat
        {
            get => camera.Parameters[PLCamera.PixelFormat].GetValue();
            set
            {
                camera.Parameters[PLCamera.PixelFormat].SetValue(value);
                OnPropertyChanged();
            }
        }
        #endregion

        #region public methods

        public void Open()
        {
            if (camera == null)
            {
                camera = new Camera();
            }

            if (!camera.IsOpen)
            {
                camera.Open();
            }
        }

        public void Close()
        {
            if (camera.IsOpen)
            {
                camera.Close();
            }
        }

        public void Start()
        {
            if (camera.StreamGrabber.IsGrabbing)
            {
                return;
            }

            camera.StreamGrabber.ImageGrabbed += OnImageRecived;
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }

        public void Stop()
        {
            camera.StreamGrabber.ImageGrabbed -= OnImageRecived;
            camera.StreamGrabber.Stop();
        }

        public void Snapshot()
        {
            savedImages.Add(lastImage);
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

            var files = Directory.GetFiles(WorkingDirectory).Select(Path.GetFileNameWithoutExtension);
            var name = "video";
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

            using (var fileWriter = new VideoFileWriter())
            {
                var fileName = Path.Combine(WorkingDirectory, $"{name}{index}.avi");
                fileWriter.Open(fileName, lastImage.Width, lastImage.Height, 25, VideoCodec.Raw);
                foreach(var bm in savedImages)
                {
                    fileWriter.WriteVideoFrame(bm);
                }

                fileWriter.Close();
            }
        }
        #endregion

        #region events handlers

        private void OnImageRecived(object sender, ImageGrabbedEventArgs e)
        {
            var bitmap = Convert(e.GrabResult);
            lastImage = bitmap.Clone() as Bitmap;
            ImageGrabbed?.Invoke(sender, new CameraBitmapEventArgs(bitmap));
        }

        #endregion

        #region helpers

        private Bitmap Convert(IGrabResult grabResult)
        {
            var bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            // Lock the bits of the bitmap.
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bmpData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Place the pointer to the buffer of the bitmap.
            converter.OutputPixelFormat = PixelType.BGRA8packed;
            var ptrBmp = bmpData.Scan0;
            converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        #endregion
    }
}
