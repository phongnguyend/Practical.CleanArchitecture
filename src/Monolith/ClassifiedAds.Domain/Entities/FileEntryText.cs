using System;

namespace ClassifiedAds.Domain.Entities;

public class FileEntryText : Entity<Guid>, IAggregateRoot
{
    public Guid FileEntryId { get; set; }

    public FileEntry FileEntry { get; set; }
}
