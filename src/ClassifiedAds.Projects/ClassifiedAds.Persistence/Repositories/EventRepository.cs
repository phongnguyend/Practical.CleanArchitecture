using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Persistence.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AdsDbContext dbContext)
            : base(dbContext)
        {
        }

        public Event GetEventIncludeSessions(Guid Id)
        {
            return DbSet.Include(x => x.Sessions).ThenInclude(s => s.Voters).FirstOrDefault(x => x.Id == Id);
        }
    }
}
