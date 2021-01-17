using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Email.SmtpClient
{
    public class FakeEmailNotification : IEmailNotification
    {
        public void Send(IEmailMessage emailMessage)
        {
            // do nothing
        }

        public Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
