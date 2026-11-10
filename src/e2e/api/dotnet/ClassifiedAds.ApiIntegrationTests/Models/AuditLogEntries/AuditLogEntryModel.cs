using System;

namespace ClassifiedAds.ApiIntegrationTests.Models.AuditLogEntries;

public class AuditLogEntryModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public string Action { get; set; }

    public string ObjectId { get; set; }

    public string Log { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}
