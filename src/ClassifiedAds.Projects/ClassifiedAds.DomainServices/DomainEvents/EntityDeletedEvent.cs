using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.DomainEvents
{
    public class EntityDeletedEvent<T> : IDomainEvent
        where T : Entity<Guid>
    {
        public EntityDeletedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
