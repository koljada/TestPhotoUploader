using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace TestPhotoUploader.Controllers
{
    public class PhotoController : Controller
    {
        private CloudBlobClient _blobClient = null;
        public PhotoController()
        {
            string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        // GET: Photo
        public ActionResult Index(string message)
        {
            return View(message);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase photo)
        {
            string message = "File is empty.";
            if (photo != null && photo.ContentLength > 0)
            {
                try
                {
                    CloudBlobContainer container = _blobClient.GetContainerReference("test-photo-analyzing");
                    container.CreateIfNotExists();

                    string folderName = GetTodayFolder();
                    CloudBlobDirectory todayFolder = container.GetDirectoryReference(folderName);

                    string name = CreateBlobName(photo);
                    CloudBlockBlob blockBlob = todayFolder.GetBlockBlobReference(name);

                    blockBlob.UploadFromStream(photo.InputStream);
                    message = $"Photo {name} was uploaded";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            return View(nameof(Index), model: message);
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