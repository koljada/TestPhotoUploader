using Microsoft.Azure;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestPhotoUploader.Models;

namespace TestPhotoUploader.Controllers
{
    public class PhotoController : Controller
    {
        private readonly CloudBlobClient _blobClient = null;
        private readonly VisionServiceClient _visionClient = null;
        private readonly CloudTableClient _tableClient = null;
        private readonly CloudQueueClient _queueClient = null;
        private readonly QueueClient _serviceBusClient = null;

        public PhotoController()
        {
            string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _tableClient = storageAccount.CreateCloudTableClient();
            _queueClient = storageAccount.CreateCloudQueueClient();

            string visionAPIKey = CloudConfigurationManager.GetSetting("VisionAPIKey");
            _visionClient = new VisionServiceClient(visionAPIKey, "https://westeurope.api.cognitive.microsoft.com/vision/v1.0");

            string serviceBusConnectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
            _serviceBusClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, "photoanalyzingresults");
        }

        // GET: Photo
        public ActionResult Index()
        {
            CloudTable table = _tableClient.GetTableReference("photoInfo");
            table.CreateIfNotExists();

            TableQuery<PhotoAnalysisResult> query = new TableQuery<PhotoAnalysisResult>().Take(10);
            var photos = table.ExecuteQuery(query).OrderByDescending(x => x.Timestamp);
            return View(photos);
        }

        public ActionResult Upload() => View();

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
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

                        var model = new PhotoAnalysisResult(result, blobTask.Result);

                        SaveData(model);

                        SendMessage(model);

                        SendMessageToBus(model);
                    }
                }
                catch (Exception ex)
                {
                    return View(model: ex.Message);
                }
            }
            else
            {
                return View(model: "File is empty.");
            }

            return RedirectToAction(nameof(Index));
        }

        //TODO: Move it to separate service
        private void SendMessageToBus(PhotoAnalysisResult model) {
            var str = JsonConvert.SerializeObject(model);
            var message = new BrokeredMessage(str);
            _serviceBusClient.Send(message);
        }

        //TODO: Move it to separate service
        private void SendMessage(PhotoAnalysisResult model)
        {
            CloudQueue queue = _queueClient.GetQueueReference("testqueueu");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage($"A new photo was uploaded and analyzed(Description: {model.Description}. Uri: {model.Uri}).");
            queue.AddMessage(message);
        }

        //TODO: Move it to separate service
        private TableResult SaveData(PhotoAnalysisResult model)
        {
            CloudTable table = _tableClient.GetTableReference("photoInfo");
            table.CreateIfNotExists();
            TableOperation insertOperation = TableOperation.Insert(model);

            return table.Execute(insertOperation);
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
            photo.InputStream.Seek(0, SeekOrigin.Begin);

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