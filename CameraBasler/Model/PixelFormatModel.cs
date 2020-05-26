using System.Collections.Generic;

namespace CameraBasler.Model
{
    public class PixelFormatModel : Model
    {
        private CameraModel CameraModel { get; }

        public IEnumerable<string> PixelFormats => CameraModel.PixelFormats;

        public string PixelFormat
        {
            get => CameraModel.PixelFormat;
            set
            {
                CameraModel.PixelFormat = value;
                OnPropertyChanged();
            }
        }

        public PixelFormatModel(CameraModel cameraModel)
        {
            CameraModel = cameraModel;
        }
    }
}
