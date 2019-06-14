using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Services
{
    public interface ISessionService : IGenericService<Session>
    {
        IList<Session> SearchSessions(string name);
    }
}
