using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Modules.Storage.Entities;

namespace ClassifiedAds.Modules.Storage.DTOs;

public class FileUploadedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
