using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Storage.Entities;

namespace ClassifiedAds.Modules.Storage.DTOs;

public class FileDeletedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
