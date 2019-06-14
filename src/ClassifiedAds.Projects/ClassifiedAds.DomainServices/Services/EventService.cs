using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassifiedAds.DomainServices.Services
{
    public class EventService : GenericService<Event>, IEventService
    {
        public EventService(IUnitOfWork unitOfWork, IRepository<Event> eventRepository) : base(unitOfWork, eventRepository)
        {

        }

        public Event GetEventIncludeSessions(Guid Id)
        {
            return _repository.GetAll().Include(x => x.Sessions).ThenInclude(s => s.Voters).Where(x => x.Id == Id).FirstOrDefault();
        }
    }
}
