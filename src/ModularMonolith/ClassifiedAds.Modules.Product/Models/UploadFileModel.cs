using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Modules.Product.Models;

public class UploadFileModel
{
    public IFormFile FormFile { get; set; }
}
