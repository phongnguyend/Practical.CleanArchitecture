using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Notification.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Repositories;

public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
{
}
