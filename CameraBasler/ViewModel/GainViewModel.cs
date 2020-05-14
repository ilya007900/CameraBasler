using CameraBasler.Model;

namespace CameraBasler.ViewModel
{
    public class GainViewModel : ViewModel
    {
        private CameraModel CameraModel { get; }

        public double GainMin => CameraModel.GainMin;

        public double GainMax => CameraModel.GainMax;

        public double Gain
        {
            get => CameraModel.Gain;
            set
            {
                if (value >= GainMin && value <= GainMax)
                {
                    CameraModel.Gain = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAutoMode
        {
            get => CameraModel.GainAuto;
            set
            {
                CameraModel.GainAuto = value;
                OnPropertyChanged();
            }
        }

        public GainViewModel(CameraModel cameraModel)
        {
            CameraModel = cameraModel;
        }
    }
}
