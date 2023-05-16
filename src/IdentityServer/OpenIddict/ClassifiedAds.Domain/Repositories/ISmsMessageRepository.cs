using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Repositories;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
}
