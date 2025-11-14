using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.FileEntries.MessageBusEvents;

public class FileDeletedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.FileEntryDeleted;

    public FileEntry FileEntry { get; set; }
}
