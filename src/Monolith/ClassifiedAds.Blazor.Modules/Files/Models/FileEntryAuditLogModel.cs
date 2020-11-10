using System;
using System.Collections.Generic;

namespace ClassifiedAds.Blazor.Modules.Files.Models
{
    public class FileEntryAuditLogModel
    {
        public string UserName { get; set; }

        public string Action { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public Dictionary<string, bool> HighLight { get; set; }

        public FileEntryModel Data { get; set; }
    }
}
