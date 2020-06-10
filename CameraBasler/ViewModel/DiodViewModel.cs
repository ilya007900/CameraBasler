using CameraBasler.Model;
using System.Windows.Media;

namespace CameraBasler.ViewModel
{
    public class DiodViewModel : ViewModel
    {
        private DiodModel diodModel;
        private SolidColorBrush color;

        public DiodModel DiodModel
        {
            get => diodModel;
            set
            {
                diodModel = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
    }
}
