using Azure.Communication.Sms;
using ClassifiedAds.Domain.Notification;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Azure;

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
            from: _options.FromNumber,
            to: smsMessage.PhoneNumber,
            message: smsMessage.Message,
            cancellationToken: cancellationToken);

        if (!string.IsNullOrWhiteSpace(response?.Value?.MessageId))
        {
        }
    }
}
