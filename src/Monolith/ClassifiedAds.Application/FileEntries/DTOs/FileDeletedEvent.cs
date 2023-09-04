using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;

namespace ClassifiedAds.Application.FileEntries.DTOs;

public class FileDeletedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
