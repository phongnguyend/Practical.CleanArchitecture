using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Events
{
    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : Entity<Guid>
    {
        public EntityUpdatedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
