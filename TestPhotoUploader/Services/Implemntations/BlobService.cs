using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class BlobService : IBlobService
    {
        private readonly CloudBlobClient _blobClient = null;

        //public BlobService()
        //{
        //    string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        //    _blobClient = storageAccount.CreateCloudBlobClient();
        //}

        public BlobService(CloudStorageAccount cloudStorageAccount)
        {
            _blobClient = cloudStorageAccount.CreateCloudBlobClient();
        }


        //TODO: Add exceptions handling
        public CloudBlockBlob UploadPhoto(HttpPostedFileBase photo)
        {
            CloudBlockBlob blockBlob = CreateBlockBlob(photo);

            blockBlob.UploadFromStream(photo.InputStream);

            return blockBlob;
        }

        public Task<CloudBlockBlob> UploadPhotoAsync(HttpPostedFileBase photo)
        {
            TaskCompletionSource<CloudBlockBlob> tcs = new TaskCompletionSource<CloudBlockBlob>();
            object state = new object();

            CloudBlockBlob blockBlob = CreateBlockBlob(photo);

            blockBlob.BeginUploadFromStream(photo.InputStream, x => tcs.SetResult(blockBlob), state);

            return tcs.Task;
        }

        private CloudBlockBlob CreateBlockBlob(HttpPostedFileBase photo)
        {
            photo.InputStream.Seek(0, SeekOrigin.Begin);

            CloudBlobContainer container = _blobClient.GetContainerReference("test-photo-analyzing");
            container.CreateIfNotExists();

            string folderName = DateTime.Today.ToString("dd-MM-yyyy");
            CloudBlobDirectory todayFolder = container.GetDirectoryReference(folderName);

            string name = CreateBlobName(photo);
            CloudBlockBlob blockBlob = todayFolder.GetBlockBlobReference(name);

            return blockBlob;
        }

        private string CreateBlobName(HttpPostedFileBase photo)
        {
            string fullName = Path.GetFileName(photo.FileName);
            string name = Path.GetFileNameWithoutExtension(fullName);
            string extension = Path.GetExtension(fullName);
            return $"{name}({Guid.NewGuid()}){extension}";
        }
    }
}