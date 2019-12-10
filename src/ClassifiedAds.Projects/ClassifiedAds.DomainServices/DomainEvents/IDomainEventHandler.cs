using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.DomainEvents
{
    public interface IDomainEventHandler<T>
           where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}
