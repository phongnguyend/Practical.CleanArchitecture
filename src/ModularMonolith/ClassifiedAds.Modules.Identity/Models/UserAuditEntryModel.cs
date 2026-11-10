using System;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Identity.Models;

public class UserAuditEntryModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public UserAuditSnapshot Data { get; set; }
    public Dictionary<string, bool> HighLight { get; set; }
}

