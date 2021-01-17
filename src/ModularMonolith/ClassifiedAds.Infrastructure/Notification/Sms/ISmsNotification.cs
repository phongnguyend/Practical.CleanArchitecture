using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Sms
{
    public interface ISmsNotification
    {
        void Send(ISmsMessage smsMessage);

        Task SendAsync(ISmsMessage smsMessage, CancellationToken cancellationToken = default);
    }

    public interface ISmsMessage
    {
        public string Message { get; set; }

        public string PhoneNumber { get; set; }
    }
}
