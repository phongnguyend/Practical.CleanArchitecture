using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Services.Configuration.Models;

public class UploadFileModel
{
    public IFormFile FormFile { get; set; }
}
