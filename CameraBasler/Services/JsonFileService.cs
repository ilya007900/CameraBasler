using CameraBasler.Interfaces;
using CameraBasler.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CameraBasler.Services
{
    public class JsonFileService : IFileService
    {
        public List<DiodModel> Open(string filename)
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<List<DiodModel>>(json);
        }

        public void Save(string filename, List<DiodModel> diods)
        {
            var json = JsonConvert.SerializeObject(diods);
            File.WriteAllText(filename, json);
        }
    }
}
