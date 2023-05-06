using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Modules.Configuration.Models;

public class UploadFileModel
{
    public IFormFile FormFile { get; set; }
}
