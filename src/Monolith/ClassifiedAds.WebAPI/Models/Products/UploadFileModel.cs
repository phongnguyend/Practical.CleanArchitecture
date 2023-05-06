using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.WebAPI.Models.Products;

public class UploadFileModel
{
    public IFormFile FormFile { get; set; }
}
