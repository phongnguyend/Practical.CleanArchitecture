using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Modules.Storage.Entities;

namespace ClassifiedAds.Modules.Storage.DTOs;

public class FileUploadedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
