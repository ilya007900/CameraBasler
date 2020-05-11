using CameraBasler.Model;

namespace CameraBasler.ViewModel
{
    public class CameraViewModel
    {
        public readonly CameraModel model;

        public string Name => model.Name;

        public CameraViewModel(CameraModel cameraModel)
        {
            model = cameraModel;
        }
    }
}
