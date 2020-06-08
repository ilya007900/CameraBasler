using CameraBasler.Model;
using System.Windows.Media;

namespace CameraBasler.ViewModel
{
    public class DiodViewModel : ViewModel
    {
        private DiodModel diodModel;
        private byte id;
        private bool isLightUp;
        private SolidColorBrush colorBrush;

        public DiodModel DiodModel
        {
            get => diodModel;
            set
            {
                diodModel = value;
                OnPropertyChanged();
            }
        }

        public byte Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public bool IsLightUp
        {
            get => isLightUp;
            set
            {
                isLightUp = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush ColorBrush
        {
            get => colorBrush;
            set
            {
                colorBrush = value;
                OnPropertyChanged();
            }
        }
    }
}
