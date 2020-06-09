using CameraBasler.Model;
using System.Collections.Generic;

namespace CameraBasler.Interfaces
{
    public interface IFileService
    {
        List<DiodModel> Open(string filename);

        void Save(string filename, List<DiodModel> diods);
    }
}
