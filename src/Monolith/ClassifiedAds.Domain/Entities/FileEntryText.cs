using System;

namespace ClassifiedAds.Domain.Entities;

public class FileEntryText : Entity<Guid>, IAggregateRoot
{
    public string TextLocation { get; set; }

    public Guid FileEntryId { get; set; }

    public FileEntry FileEntry { get; set; }
}
