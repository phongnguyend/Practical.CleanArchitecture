using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Storage.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.EventHandlers
{
    public class FileEntryUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<FileEntry>>
    {
        private readonly IServiceProvider _serviceProvider;

        public FileEntryUpdatedEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync(EntityUpdatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
        {
            // Handle the event here and we can also forward to external systems
            using (var scope = _serviceProvider.CreateScope())
            {
                var auditSerivce = scope.ServiceProvider.GetService<IAuditLogService>();
                var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

                await auditSerivce.AddOrUpdateAsync(new AuditLogEntryDTO
                {
                    UserId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty,
                    CreatedDateTime = domainEvent.EventDateTime,
                    Action = "UPDATED_FILEENTRY",
                    ObjectId = domainEvent.Entity.Id.ToString(),
                    Log = domainEvent.Entity.AsJsonString(),
                });
            }
        }
    }
}
