using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace TestPhotoUploader.Models
{
    public class PhotoAnalysisResult : TableEntity
    {
        public PhotoAnalysisResult() { }

        public PhotoAnalysisResult(AnalysisResult result, CloudBlockBlob photo) 
            : base(
                  photo.Name.Split('/','(')[1], 
                  result.RequestId.ToString())
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddYears(1),
                Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
            };
            string signature = photo.GetSharedAccessSignature(policy);

            Uri = $"{photo.Container.Uri}/{photo.Name}{signature}";

            Name = photo.Name;
            Description = result.Description?.Captions?.FirstOrDefault()?.Text;

            AccentColor = result.Color?.AccentColor;
            DominantColorForeground = result.Color?.DominantColorForeground;
            DominantColorBackground = result.Color?.DominantColorBackground;

            var tags = result.Tags?.OrderByDescending(x => x.Confidence)?.ToList();
            if (tags != null && tags.Any())
            {
                Tag1 = tags[0].Name;

                if (tags.Count > 1)
                    Tag2 = tags[1].Name;
                if (tags.Count > 2)
                    Tag3 = tags[3].Name;
                if (tags.Count > 3)
                    Tag4 = tags[3].Name;
                if (tags.Count > 4)
                    Tag5 = tags[4].Name;
            }

            try
            {
                Json = JsonConvert.SerializeObject(result);
            }
            catch (Exception)
            { }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Uri { get; set; }

        public string AccentColor { get; set; }
        public string DominantColorForeground { get; set; }
        public string DominantColorBackground { get; set; }

        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Tag3 { get; set; }
        public string Tag4 { get; set; }
        public string Tag5 { get; set; }

        public string Json { get; set; }
    }
}