using CameraBasler.ViewModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace CameraBasler.View
{
    /// <summary>
    /// Interaction logic for CameraPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly MainPageViewModel viewModel;

        public MainPage()
        {
            viewModel = new MainPageViewModel();
            DataContext = viewModel;

            InitializeComponent();
        }

        private void CheckNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(x => char.IsDigit(x));
        }
    }
}
