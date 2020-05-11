using Basler.Pylon;
using System;

namespace CameraBasler.Model
{
    public class CameraModel
    {
        private const string ExposureAutoModeOn = "Continuous";
        private const string ExposureAutoModeOff = "Off";
        private const string GainAutoModeOn = "Continuous";
        private const string GainAutoModeOff = "Off";
        private const string FriendlyNameKey = "FriendlyName";

        private readonly ICamera camera;

        public event EventHandler<ImageGrabbedEventArgs> ImageGrabbed;

        #region public properties

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

        public CameraModel()
        {
            camera = new Camera();
        }

        #region public methods

        public void Open()
        {
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
            camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
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
            ImageGrabbed?.Invoke(sender, e);
        }

        #endregion
    }
}
