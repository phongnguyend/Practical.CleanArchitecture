using Azure.Communication;
using Azure.Communication.Sms;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Azure
{
    public class AzureSmsNotification : ISmsNotification
    {
        private readonly AzureOptions _options;

        public AzureSmsNotification(AzureOptions options)
        {
            _options = options;
        }

        public void Send(SmsMessageDTO smsMessage)
        {
            var smsClient = new SmsClient(_options.ConnectionString);
            var response = smsClient.Send(
                from: new PhoneNumber(_options.FromNumber),
                to: new PhoneNumber(smsMessage.PhoneNumber),
                message: smsMessage.Message);

            if (!string.IsNullOrWhiteSpace(response?.Value?.MessageId))
            {
            }
        }
    }
}
