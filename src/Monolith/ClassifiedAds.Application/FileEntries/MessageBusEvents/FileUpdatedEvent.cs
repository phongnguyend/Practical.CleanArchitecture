using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.FileEntries.MessageBusEvents;

public class FileUpdatedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.FileEntryUpdated;

    public FileEntry FileEntry { get; set; }
}
