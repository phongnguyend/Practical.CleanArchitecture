using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Notification;

public interface ISmsNotification
{
    Task SendAsync(ISmsMessage smsMessage, CancellationToken cancellationToken = default);
}

public interface ISmsMessage
{
    public string Message { get; set; }

    public string PhoneNumber { get; set; }
}
