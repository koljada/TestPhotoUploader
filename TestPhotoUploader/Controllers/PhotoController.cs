using Microsoft.Azure;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TestPhotoUploader.Controllers
{
    public class PhotoController : Controller
    {
        private CloudBlobClient _blobClient = null;
        private VisionServiceClient _visionClient = null;

        public PhotoController()
        {
            string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();

            string visionAPIKey = CloudConfigurationManager.GetSetting("VisionAPIKey");
            _visionClient = new VisionServiceClient(visionAPIKey, "https://westeurope.api.cognitive.microsoft.com/vision/v1.0");
        }

        // GET: Photo
        public ActionResult Index(string message)
        {
            return View(message);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
            string message = "File is empty.";
            if (photo != null && photo.ContentLength > 0)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream())
                    {
                        photo.InputStream.CopyTo(copyStream);

                        var blobTask = UploadPhotoAsync(photo);

                        var analysisTask = AnalyzeImageAsync(copyStream);

                        await Task.WhenAll(blobTask, analysisTask);

                        var result = analysisTask.Result;
                        //TODO: Implement ToString and saving results. Add a separate model
                        message = $"Photo {blobTask.Result.Name} was uploaded. " +
                            $"Description: {string.Join(",", result.Description.Captions.Select(x => x.Text))}";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            return View(nameof(Index), model: message);
        }

        //TODO: Move it to separate service
        private async Task<AnalysisResult> AnalyzeImageAsync(Stream photoStream)
        {
            photoStream.Seek(0, SeekOrigin.Begin);

            var parameters = new List<VisualFeature> {
                    //VisualFeature.Adult,
                    //VisualFeature.Categories,
                    VisualFeature.Color,
                    VisualFeature.Description,
                    //VisualFeature.Faces,
                    //VisualFeature.ImageType,
                    VisualFeature.Tags
                };

            return await _visionClient.AnalyzeImageAsync(photoStream, parameters, null);
        }

        //TODO: Move it to separate service
        private async Task<CloudBlockBlob> UploadPhotoAsync(HttpPostedFileBase photo)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference("test-photo-analyzing");
            container.CreateIfNotExists();

            string folderName = GetTodayFolder();
            CloudBlobDirectory todayFolder = container.GetDirectoryReference(folderName);

            string name = CreateBlobName(photo);
            CloudBlockBlob blockBlob = todayFolder.GetBlockBlobReference(name);

            await blockBlob.UploadFromStreamAsync(photo.InputStream);

            return blockBlob;
        }

        private string GetTodayFolder() => DateTime.Today.ToString("dd-MM-yyyy");

        private string CreateBlobName(HttpPostedFileBase photo)
        {
            string fullName = Path.GetFileName(photo.FileName);
            string name = Path.GetFileNameWithoutExtension(fullName);
            string extension = Path.GetExtension(fullName);
            return $"{name}({Guid.NewGuid()}){extension}";
        }
    }
}