using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.WebAPI.Models.ConfigurationEntries;

public class UploadFileModel
{
    public IFormFile FormFile { get; set; }
}
