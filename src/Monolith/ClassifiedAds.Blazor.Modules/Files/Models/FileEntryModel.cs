using System;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Blazor.Modules.Files.Models
{
    public class FileEntryModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public long Size { get; set; }

        public DateTimeOffset UploadedTime { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }
    }
}
