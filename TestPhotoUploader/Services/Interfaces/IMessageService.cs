using System.Threading.Tasks;

namespace TestPhotoUploader.Services.Interfaces
{
    public interface IMessageService<T>
    {
        /// <summary>
        /// Send a message to the queue
        /// </summary>
        /// <param name="entity"></param>
        void SendMessage(T entity);

        /// <summary>
        /// Send a message to the queue asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task SendMessageAsync(T entity);
    }
}