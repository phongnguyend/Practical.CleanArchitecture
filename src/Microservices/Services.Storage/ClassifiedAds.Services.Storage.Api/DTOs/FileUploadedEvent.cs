using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class FileUploadedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
