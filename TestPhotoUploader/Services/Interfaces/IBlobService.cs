using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.Web;

namespace TestPhotoUploader.Services.Interfaces
{
    public interface IBlobService
    {
        /// <summary>
        /// Upload a photo to the blob storage asynchronously
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        Task<CloudBlockBlob> UploadPhotoAsync(HttpPostedFileBase photo);

        /// <summary>
        /// Upload a photo to the blob storage
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        CloudBlockBlob UploadPhoto(HttpPostedFileBase photo);
    }
}