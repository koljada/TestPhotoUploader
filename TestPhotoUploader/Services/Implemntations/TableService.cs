using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Services.Implemntations
{
    public class TableService<T> : ITableService<T> where T : TableEntity, new()
    {
        private readonly CloudTableClient _tableClient = null;

        //public TableService()
        //{
        //    string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        //    _tableClient = storageAccount.CreateCloudTableClient();
        //}

        public TableService(CloudStorageAccount cloudStorageAccount)
        {
            _tableClient = cloudStorageAccount.CreateCloudTableClient();
        }

        public IEnumerable<T> GetAll()
        {
            CloudTable table = _tableClient.GetTableReference("photoInfo");
            table.CreateIfNotExists();

            TableQuery<T> query = new TableQuery<T>();
            var photos = table.ExecuteQuery(query);

            return photos;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
            //var items = new List<T>();
            //TableContinuationToken token = null;
            //CancellationToken ct = default(CancellationToken);

            //CloudTable table = _tableClient.GetTableReference("photoInfo");
            //table.CreateIfNotExists();

            //TableQuery<T> query = new TableQuery<T>();

            //do
            //{
            //    ICancellableAsyncResult ar = table.BeginExecuteQuerySegmented(query, token, null, null);
            //    ct.Register(ar.Cancel);

            //    TableQuerySegment<T> seg = Task.Factory.FromAsync(ar, table.EndExecuteQuerySegmented<T>).Result;
            //    token = seg.ContinuationToken;
            //    items.AddRange(seg);
            //} while (token != null && !ct.IsCancellationRequested);

            //return items;
        }

        public TableResult Insert(T entity)
        {
            CloudTable table = _tableClient.GetTableReference("photoInfo");
            table.CreateIfNotExists();
            TableOperation insertOperation = TableOperation.Insert(entity);

            var tableResult = table.Execute(insertOperation);

            return tableResult;
        }

        public Task<TableResult> InsertAsync(T entity)
        {
            TaskCompletionSource<TableResult> tcs = new TaskCompletionSource<TableResult>();
            object state = new object();

            CloudTable table = _tableClient.GetTableReference("photoInfo");
            table.CreateIfNotExists();
            TableOperation insertOperation = TableOperation.Insert(entity);

            table.BeginExecute(insertOperation, x => tcs.SetResult(table.EndExecute(x)), state);

            return tcs.Task;
        }
    }
}