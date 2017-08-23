using Microsoft.WindowsAzure.Storage.Table;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class AzureService<T> : IAzureService<T> where T : TableEntity, new()
    {
        private readonly IBlobService _blobService = null;
        private readonly ITableService<T> _tableService = null;
        private readonly IMessageService<T> _messageService = null;

        public AzureService(IBlobService blobService, ITableService<T> tableService, IMessageService<T> messageService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _messageService = messageService;
        }

        public IBlobService BlobService => _blobService;

        public ITableService<T> TableService => _tableService;

        public IMessageService<T> MessageService => _messageService;
    }
}