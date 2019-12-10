using ClassifiedAds.DomainServices.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Services
{
    public interface IEventService : IGenericService<Event>
    {
        Event GetEventIncludeSessions(Guid Id);
    }
}
