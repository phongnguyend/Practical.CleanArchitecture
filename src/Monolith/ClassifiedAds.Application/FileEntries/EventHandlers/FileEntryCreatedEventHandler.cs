using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task HandleAsync(EntityCreatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditSerivce = scope.ServiceProvider.GetService<ICrudService<AuditLogEntry>>();
                var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

                await auditSerivce.AddOrUpdateAsync(new AuditLogEntry
                {
                    UserId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty,
                    CreatedDateTime = domainEvent.EventDateTime,
                    Action = "CREATED_FILEENTRY",
                    ObjectId = domainEvent.Entity.Id.ToString(),
                    Log = domainEvent.Entity.AsJsonString(),
                });
            }

            // Forward to external systems
            await _fileUploadedEventSender.SendAsync(new FileUploadedEvent
            {
                FileEntry = domainEvent.Entity,
            });
        }
    }
}
