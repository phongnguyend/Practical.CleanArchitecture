using ClassifiedAds.DomainServices.Entities;
using System;

namespace ClassifiedAds.DomainServices.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Event GetEventIncludeSessions(Guid Id);
    }
}
