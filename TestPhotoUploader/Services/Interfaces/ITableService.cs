using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestPhotoUploader.Services.Interfaces
{
    public interface ITableService<T> where T : TableEntity, new()
    {
        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TableResult Insert(T entity);

        /// <summary>
        /// Insert a new entity asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TableResult> InsertAsync(T entity);

        /// <summary>
        /// Get all entities from the table
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get all entities from the table asynchronously
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
    }
}