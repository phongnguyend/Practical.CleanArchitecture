using System;

namespace ClassifiedAds.Application.AuditLogEntries.DTOs
{
    public class AuditLogEntryDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Action { get; set; }

        public string ObjectId { get; set; }

        public string Log { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
