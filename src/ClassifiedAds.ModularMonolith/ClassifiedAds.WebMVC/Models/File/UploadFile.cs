using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.WebMVC.Models.File
{
    public class UploadFile
    {
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 0)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
