using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Events
{
    public class EntityCreatedEvent<T> : IDomainEvent
        where T : Entity<Guid>
    {
        public EntityCreatedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
