using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Threading.Tasks;

namespace TestPhotoUploader.Services.Interfaces
{
    public interface ICognitiveService
    {
        /// <summary>
        /// Analyze an image asynchronously
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<AnalysisResult> AnalyzeImageAzync(Stream stream);

        /// <summary>
        /// Analyze an image
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        AnalysisResult AnalyzeImage(Stream stream);
    }
}