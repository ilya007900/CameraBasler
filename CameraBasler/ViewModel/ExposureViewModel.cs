using CameraBasler.Model;

namespace CameraBasler.ViewModel
{
    public class ExposureViewModel : ViewModel
    {
        private CameraModel CameraModel { get; }

        public double ExposureMin => CameraModel.ExposureTimeMin;

        public double ExposureMax => CameraModel.ExposureTimeMax;

        public double Exposure
        {
            get => CameraModel.ExposureTime ;
            set
            {
                if (value >= ExposureMin && value <= ExposureMax)
                {
                    CameraModel.ExposureTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAutoMode
        {
            get => CameraModel.ExposureAuto;
            set
            {
                CameraModel.ExposureAuto = value;
                OnPropertyChanged();
            }
        }

        public ExposureViewModel(CameraModel cameraModel)
        {
            CameraModel = cameraModel;
        }
    }
}
