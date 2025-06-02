using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Modules.Storage.Entities;

namespace ClassifiedAds.Modules.Storage.DTOs;

public class FileDeletedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
