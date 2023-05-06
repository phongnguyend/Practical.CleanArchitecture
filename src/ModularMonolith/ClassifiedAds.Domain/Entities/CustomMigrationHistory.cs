using System;

namespace ClassifiedAds.Domain.Entities;

public class CustomMigrationHistory : Entity<Guid>
{
    public string MigrationName { get; set; }
}
