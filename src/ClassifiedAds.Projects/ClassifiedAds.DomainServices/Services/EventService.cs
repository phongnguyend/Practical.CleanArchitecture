using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Repositories;
using System;
using System.Linq;

namespace ClassifiedAds.DomainServices.Services
{
    public class EventService : GenericService<Event>, IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IUnitOfWork unitOfWork, IEventRepository eventRepository)
            : base(unitOfWork, eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Event GetEventIncludeSessions(Guid Id)
        {
            return _eventRepository.GetEventIncludeSessions(Id);
        }
    }
}
