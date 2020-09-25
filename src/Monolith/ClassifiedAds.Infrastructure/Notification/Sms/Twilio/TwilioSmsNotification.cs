using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Notification;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Twilio
{
    public class TwilioSmsNotification : ISmsNotification
    {
        private readonly TwilioOptions _options;

        public TwilioSmsNotification(TwilioOptions options)
        {
            _options = options;
        }

        public void Send(SmsMessage smsMessage)
        {
            TwilioClient.Init(_options.AccountSId, _options.AuthToken);

            var message = MessageResource.Create(
                body: smsMessage.Message,
                from: new PhoneNumber(_options.FromNumber),
                to: new PhoneNumber(smsMessage.PhoneNumber));

            if (!string.IsNullOrWhiteSpace(message.Sid))
            {
            }
        }
    }
}
