using ClassifiedAds.Domain.Notification;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Twilio;

public class TwilioSmsNotification : ISmsNotification
{
    private readonly TwilioOptions _options;

    public TwilioSmsNotification(TwilioOptions options)
    {
        _options = options;
    }

    public async Task SendAsync(ISmsMessage smsMessage, CancellationToken cancellationToken = default)
    {
        TwilioClient.Init(_options.AccountSId, _options.AuthToken);

        var message = await MessageResource.CreateAsync(
            body: smsMessage.Message,
            from: new PhoneNumber(_options.FromNumber),
            to: new PhoneNumber(smsMessage.PhoneNumber));

        if (!string.IsNullOrWhiteSpace(message.Sid))
        {
        }
    }
}
