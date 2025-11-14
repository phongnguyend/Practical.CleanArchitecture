using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.FileEntries.MessageBusEvents;

public class FileCreatedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.FileEntryCreated;

    public FileEntry FileEntry { get; set; }
}
