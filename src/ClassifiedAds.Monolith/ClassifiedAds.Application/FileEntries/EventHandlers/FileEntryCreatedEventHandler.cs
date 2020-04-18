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
    public class FileEntryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<FileEntry>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageSender<FileUploadedEvent> _fileUploadedEventSender;

        public FileEntryCreatedEventHandler(IMessageSender<FileUploadedEvent> fileUploadedEventSender, IServiceProvider serviceProvider)
        {
            _fileUploadedEventSender = fileUploadedEventSender;
            _serviceProvider = serviceProvider;
        }

        public void Handle(EntityCreatedEvent<FileEntry> domainEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditSerivce = scope.ServiceProvider.GetService<ICrudService<AuditLogEntry>>();
                var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

                auditSerivce.AddOrUpdate(new AuditLogEntry
                {
                    UserId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty,
                    CreatedDateTime = domainEvent.EventDateTime,
                    Action = "CREATED_FILEENTRY",
                    ObjectId = domainEvent.Entity.Id.ToString(),
                    Log = domainEvent.Entity.AsJsonString(),
                });
            }

            // Forward to external systems
            _fileUploadedEventSender.Send(new FileUploadedEvent
            {
                FileEntry = domainEvent.Entity,
            });
        }
    }
}
