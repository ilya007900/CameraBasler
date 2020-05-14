using CameraBasler.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CameraBasler.View
{
    /// <summary>
    /// Interaction logic for CameraPage.xaml
    /// </summary>
    public partial class CameraPage : Page
    {
        private readonly CameraViewModel viewModel;

        public CameraPage()
        {
            viewModel = new CameraViewModel();
            DataContext = viewModel;

            InitializeComponent();
        }

        private void CameraState_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.IsCameraOpen)
            {
                viewModel.CloseCamera();
                CameraState.Content = "Off";
                CameraState.Background = Brushes.Red;
            }
            else
            {
                try
                {
                    viewModel.OpenCamera();
                    CameraState.Content = "On";
                    CameraState.Background = Brushes.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartGrab();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StopGrab();
        }

        private void CheckNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(x => char.IsDigit(x));
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ArduinoViewModel.Connect();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ArduinoViewModel.Disconnect();
        }

        private void StartEyeButton_Click(object sender, RoutedEventArgs e)
        {
            EyeState.Content = "In progress";
            viewModel.PupilReactionViewModel.InProgress = true;
        }

        private void StopEyeButton_Click(object sender, RoutedEventArgs e)
        {
            EyeState.Content = "Finished";
            viewModel.PupilReactionViewModel.InProgress = false;
        }

        private void RefreshPortsButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ArduinoViewModel.RefreshPorts();
        }

        private void ExecuteCommandButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ArduinoViewModel.WriteCommand();
        }
    }
}
