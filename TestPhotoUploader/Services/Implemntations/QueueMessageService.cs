using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class QueueMessageService<T> : IMessageService<T> where T : TableEntity, new()
    {
        private readonly CloudQueueClient _queueClient = null;

        //public QueueMessageService()
        //{
        //    string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        //    _queueClient = storageAccount.CreateCloudQueueClient();
        //}

        public QueueMessageService(CloudStorageAccount cloudStorageAccount)
        {
            _queueClient = cloudStorageAccount.CreateCloudQueueClient();
        }


        public void SendMessage(T entity)
        {
            CloudQueue queue = _queueClient.GetQueueReference("testqueueu");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage($"A new photo was uploaded and analyzed({entity.PartitionKey}).");
            queue.AddMessage(message);
        }

        public Task SendMessageAsync(T entity)
        {
            CloudQueue queue = _queueClient.GetQueueReference("testqueueu");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage($"A new photo was uploaded and analyzed({entity.PartitionKey}).");
            return queue.AddMessageAsync(message);
        }
    }
}