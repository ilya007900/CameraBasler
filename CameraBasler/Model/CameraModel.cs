using Basler.Pylon;
using CameraBasler.Entities;
using CameraBasler.Events;
using Newtonsoft.Json;
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

        private readonly ICamera camera;

        private readonly PixelDataConverter converter = new PixelDataConverter();

        private byte[] lastImage;

        public event EventHandler<CameraBitmapEventArgs> ImageGrabbed;

        #region public properties

        public bool IsOpen => camera.IsOpen;

        public string Name => camera.CameraInfo["FriendlyName"];

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
                if (value >= GainMin && value <= GainMax && !GainAuto)
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

        public double FrameRate
        {
            get => camera.Parameters[PLCamera.ResultingFrameRate].GetValue();
        }

        public bool IsGrabbing
        {
            get => camera.StreamGrabber.IsGrabbing;
        }

        #endregion

        #region life cycle

        public CameraModel()
        {
            camera = new Camera();
        }

        public CameraModel(ICamera camera)
        {
            this.camera = camera;
        }

        #endregion

        #region public methods

        public void Open()
        {
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
            if (IsGrabbing)
            {
                return;
            }

            camera.StreamGrabber.ImageGrabbed += OnImageRecived;
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            OnPropertyChanged(nameof(IsGrabbing));
        }

        public void Stop()
        {
            if (!IsGrabbing)
            {
                return;
            }

            camera.StreamGrabber.ImageGrabbed -= OnImageRecived;
            camera.StreamGrabber.Stop();
            OnPropertyChanged(nameof(IsGrabbing));
        }

        public byte[] Snapshot()
        {
            return lastImage.Clone() as byte[];
        }

        #endregion

        #region events handlers

        private void OnImageRecived(object sender, ImageGrabbedEventArgs e)
        {
            OnPropertyChanged(nameof(FrameRate));
            if (ExposureAuto)
            {
                OnPropertyChanged(nameof(ExposureTime));
            }

            if (GainAuto)
            {
                OnPropertyChanged(nameof(Gain));
            }

            var bitmap = Convert(e.GrabResult);
            if (bitmap != null)
            {
                var bytes = e.GrabResult.PixelData as byte[];
                if (lastImage == null || lastImage.Length != bytes.Length)
                {
                    lastImage = new byte[bytes.Length];
                }

                bytes.CopyTo(lastImage, 0);

                ImageGrabbed?.Invoke(sender, new CameraBitmapEventArgs(bitmap));
            }
        }

        #endregion

        #region helpers

        private Bitmap Convert(IGrabResult grabResult)
        {
            if (grabResult.Width == 0 || grabResult.Height == 0)
            {
                return null;
            }

            var pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            var bitmap = new Bitmap(grabResult.Width, grabResult.Height, pixelFormat);
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bmpData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            converter.OutputPixelFormat = PixelType.BGRA8packed;

            var ptrBmp = bmpData.Scan0;
            converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        #endregion
    }
}
