using Microsoft.WindowsAzure.Storage.Table;

namespace TestPhotoUploader.Services.Interfaces
{
    public interface IAzureService<T> where T : TableEntity, new()
    {
        /// <summary>
        /// Service for blobs
        /// </summary>
        IBlobService BlobService { get; }

        /// <summary>
        /// Service for tables
        /// </summary>
        ITableService<T> TableService { get; }

        /// <summary>
        /// Service for messages
        /// </summary>
        IMessageService<T> MessageService { get; }
    }
}