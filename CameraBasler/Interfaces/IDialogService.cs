namespace CameraBasler.Interfaces
{
    public interface IDialogService
    {
        string FilePath { get; set; }

        void ShowMessage(string message);

        bool OpenFileDialog(string filter);

        bool SaveFileDialog(string filter);
    }
}
