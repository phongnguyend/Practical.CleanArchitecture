using ClassifiedAds.Domain.Infrastructure.Storages;
using System;

namespace ClassifiedAds.Domain.Entities;

public class FileEntry : Entity<Guid>, IAggregateRoot, IFileEntry
{
    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public DateTimeOffset UploadedTime { get; set; }

    public string FileName { get; set; }

    public string FileLocation { get; set; }

    public bool Encrypted { get; set; }

    public string EncryptionKey { get; set; }

    public string EncryptionIV { get; set; }

    public bool Archived { get; set; }

    public DateTimeOffset? ArchivedDate { get; set; }

    public bool Deleted { get; set; }

    public DateTimeOffset? DeletedDate { get; set; }
}

public class DeletedFileEntry : Entity<Guid>, IAggregateRoot
{
    public Guid FileEntryId { get; set; }
}
