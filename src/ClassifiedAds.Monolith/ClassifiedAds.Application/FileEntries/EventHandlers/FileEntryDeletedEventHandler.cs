using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.Application.FileEntries.EventHandlers
{
    public class FileEntryDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<FileEntry>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageSender<FileDeletedEvent> _fileDeletedEventSender;

        public FileEntryDeletedEventHandler(IMessageSender<FileDeletedEvent> fileDeletedEventSender, IServiceProvider serviceProvider)
        {
            _fileDeletedEventSender = fileDeletedEventSender;
            _serviceProvider = serviceProvider;
        }

        public void Handle(EntityDeletedEvent<FileEntry> domainEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditSerivce = scope.ServiceProvider.GetService<ICrudService<AuditLogEntry>>();
                var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

                auditSerivce.AddOrUpdate(new AuditLogEntry
                {
                    UserId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty,
                    CreatedDateTime = domainEvent.EventDateTime,
                    Action = "DELETE_FILEENTRY",
                    ObjectId = domainEvent.Entity.Id.ToString(),
                    Log = domainEvent.Entity.AsJsonString(),
                });
            }

            // Forward to external systems
            _fileDeletedEventSender.Send(new FileDeletedEvent
            {
                FileEntry = domainEvent.Entity,
            });
        }
    }
}
