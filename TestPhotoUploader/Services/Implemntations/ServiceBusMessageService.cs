using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class ServiceBusMessageService<T> : IMessageService<T> where T : TableEntity, new()
    {
        private readonly QueueClient _serviceBusClient = null;

        private const string QUEUE_NAME = "photoanalyzingresults";

        public ServiceBusMessageService()
        {
            string serviceBusConnectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
            _serviceBusClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, QUEUE_NAME);
        }


        public void SendMessage(T entity)
        {
            var message = GetMessage(entity);
            _serviceBusClient.Send(message);
        }

        public Task SendMessageAsync(T entity)
        {
            var message = GetMessage(entity);
            return _serviceBusClient.SendAsync(message);
        }

        private BrokeredMessage GetMessage(T entity) => new BrokeredMessage(JsonConvert.SerializeObject(entity));
    }
}