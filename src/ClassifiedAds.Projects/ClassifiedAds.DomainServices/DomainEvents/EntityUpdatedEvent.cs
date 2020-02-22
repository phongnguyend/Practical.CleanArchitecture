using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.DomainEvents
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
