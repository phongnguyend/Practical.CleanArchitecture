using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.EventHandlers
{
    public class ProductDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<Entities.Product>>
    {
        private readonly IServiceProvider _serviceProvider;

        public ProductDeletedEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync(EntityDeletedEvent<Entities.Product> domainEvent, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var currentUser = serviceProvider.GetService<ICurrentUser>();
                var dispatcher = serviceProvider.GetService<Dispatcher>();

                await dispatcher.DispatchAsync(new AddAuditLogEntryCommand
                {
                    AuditLogEntry = new AuditLogEntryDTO
                    {
                        UserId = currentUser.UserId,
                        CreatedDateTime = domainEvent.EventDateTime,
                        Action = "DELETED_PRODUCT",
                        ObjectId = domainEvent.Entity.Id.ToString(),
                        Log = domainEvent.Entity.AsJsonString(),
                    },
                });
            }
        }
    }
}
