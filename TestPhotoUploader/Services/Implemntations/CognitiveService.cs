using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class CognitiveService : ICognitiveService
    {
        private readonly VisionServiceClient _visionClient = null;

        public CognitiveService()
        {
            string visionAPIKey = ConfigurationManager.AppSettings["VisionAPIKey"];
            _visionClient = new VisionServiceClient(visionAPIKey, "https://westeurope.api.cognitive.microsoft.com/vision/v1.0");
        }


        public AnalysisResult AnalyzeImage(Stream stream) => Task.Run(() => AnalyzeImageAzync(stream)).Result;

        public Task<AnalysisResult> AnalyzeImageAzync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<VisualFeature> parameters = new List<VisualFeature> {
                    //VisualFeature.Adult,
                    //VisualFeature.Categories,
                    VisualFeature.Color,
                    VisualFeature.Description,
                    //VisualFeature.Faces,
                    //VisualFeature.ImageType,
                    VisualFeature.Tags
                };

            return _visionClient.AnalyzeImageAsync(stream, parameters, null);
        }
    }
}