using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.DomainEvents
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
