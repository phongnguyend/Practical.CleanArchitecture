using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class FileUploadedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
