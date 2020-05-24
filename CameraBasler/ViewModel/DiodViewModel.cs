using CameraBasler.Model;

namespace CameraBasler.ViewModel
{
    public class DiodViewModel : ViewModel
    {
        private DiodModel model;
        private byte id;
        private bool isLightUp;

        public DiodModel Model
        {
            get => model;
            set
            {
                model = value;
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

        public DiodViewModel()
        {
            Model = new DiodModel();
        }
    }
}
