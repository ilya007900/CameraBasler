using CameraBasler.View;
using System.Windows;

namespace CameraBasler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Content = new MainPage();
        }
    }
}
