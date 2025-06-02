using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.FileEntries.DTOs;

public class FileUploadedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
