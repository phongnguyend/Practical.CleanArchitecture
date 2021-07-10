using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Services.Product.Models
{
    public class UploadFileModel
    {
        public IFormFile FormFile { get; set; }
    }
}
