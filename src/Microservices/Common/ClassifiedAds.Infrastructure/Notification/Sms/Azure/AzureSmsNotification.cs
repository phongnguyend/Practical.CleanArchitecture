using Azure.Communication;
using Azure.Communication.Sms;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Azure
{
    public class AzureSmsNotification : ISmsNotification
    {
        private readonly AzureOptions _options;

        public AzureSmsNotification(AzureOptions options)
        {
            _options = options;
        }

        public async Task SendAsync(ISmsMessage smsMessage, CancellationToken cancellationToken = default)
        {
            var smsClient = new SmsClient(_options.ConnectionString);
            var response = await smsClient.SendAsync(
                from: new PhoneNumber(_options.FromNumber),
                to: new PhoneNumber(smsMessage.PhoneNumber),
                message: smsMessage.Message,
                cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(response?.Value?.MessageId))
            {
            }
        }
    }
}
