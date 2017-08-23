using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestPhotoUploader.Models;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IAzureService<PhotoAnalysisResult> _azureService = null;
        private readonly ICognitiveService _cognitiveService = null;

        public PhotoController(ICognitiveService cognitiveService, IAzureService<PhotoAnalysisResult> azureService)
        {
            _cognitiveService = cognitiveService;
            _azureService = azureService;
        }

        public ViewResult Index()
        {
            var photos = _azureService.TableService.GetAll();
            var lastPhotos = photos.OrderByDescending(x => x.Timestamp).Take(10);
            return View(lastPhotos);
        }

        public ActionResult Upload() => View();

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength > 0)
            {
                try
                {
                    using (MemoryStream copyStream = new MemoryStream())
                    {
                        photo.InputStream.CopyTo(copyStream);

                        var blobTask = _azureService.BlobService.UploadPhotoAsync(photo);

                        var analysisTask = _cognitiveService.AnalyzeImageAzync(copyStream);

                        await Task.WhenAll(blobTask, analysisTask);

                        var result = analysisTask.Result;

                        var model = new PhotoAnalysisResult(result, blobTask.Result);

                        var tableTask = _azureService.TableService.InsertAsync(model);

                        var queueTask = _azureService.MessageService.SendMessageAsync(model);
                    }
                }
                catch (Exception ex)
                {
                    return View(model: ex.Message);
                }
            }
            else
            {
                return View(model: "File is empty.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}