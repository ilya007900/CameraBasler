﻿using Basler.Pylon;
using CameraBasler.Events;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CameraBasler.Model
{
    public class CameraModel : IDisposable
    {
        private const string ExposureAutoModeOn = "Continuous";
        private const string ExposureAutoModeOff = "Off";
        private const string GainAutoModeOn = "Continuous";
        private const string GainAutoModeOff = "Off";
        private const string FriendlyNameKey = "FriendlyName";

        private ICamera camera;

        public event EventHandler<CameraBitmapEventArgs> ImageGrabbed;

        public void Dispose()
        {
            camera.Close();
            camera.Dispose();
            camera = null;
        }

        #region public properties

        public bool IsOpen => camera?.IsOpen ?? false;

        public string Name => camera.CameraInfo[FriendlyNameKey];

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
                if (value <= ExposureTimeMax && value >= ExposureTimeMin)
                {
                    camera.Parameters[PLCamera.ExposureTime].SetValue(value);
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
                var openedCamera = camera.Open();
                //camera = openedCamera;
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
            camera.StreamGrabber.ImageGrabbed += OnImageRecived;
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }

        public void Stop()
        {
            camera.StreamGrabber.ImageGrabbed -= OnImageRecived;
            camera.StreamGrabber.Stop();
        }

        #endregion

        #region events handlers

        private void OnImageRecived(object sender, ImageGrabbedEventArgs e)
        {
            var bitmap = Convert(e.GrabResult);
            ImageGrabbed?.Invoke(sender, new CameraBitmapEventArgs(bitmap));
        }

        #endregion

        #region helpers

        private static Bitmap Convert(IGrabResult grabResult)
        {
            var converter = new PixelDataConverter();
            var bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
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