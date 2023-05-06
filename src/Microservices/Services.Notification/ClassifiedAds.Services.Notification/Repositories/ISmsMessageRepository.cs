using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;
using System;

namespace ClassifiedAds.Services.Notification.Repositories;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
}
