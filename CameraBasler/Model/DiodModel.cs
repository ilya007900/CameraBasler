namespace CameraBasler.Model
{
    public class DiodModel : Model
    {
        private bool isUsing;
        private int maxEnergy;
        private double tau;
        private int km1;
        private int km2;
        private int step;

        public bool IsUsing
        {
            get => isUsing;
            set
            {
                isUsing = value;
                OnPropertyChanged();
            }
        }

        public int MaxEnergy
        {
            get => maxEnergy;
            set
            {
                maxEnergy = value;
                OnPropertyChanged();
            }
        }

        public double Tau
        {
            get => tau;
            set
            {
                tau = value;
                OnPropertyChanged();
            }
        }

        public int Km1
        {
            get => km1;
            set
            {
                km1 = value;
                OnPropertyChanged();
            }
        }

        public int Km2
        {
            get => km2;
            set
            {
                km2 = value;
                OnPropertyChanged();
            }
        }

        public int Step
        {
            get => step;
            set
            {
                step = value;
                OnPropertyChanged();
            }
        }
    }
}
