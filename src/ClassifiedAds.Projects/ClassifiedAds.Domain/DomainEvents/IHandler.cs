using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Domain.DomainEvents
{
    public interface IHandler<T>
           where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}
