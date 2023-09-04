using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class FileDeletedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
