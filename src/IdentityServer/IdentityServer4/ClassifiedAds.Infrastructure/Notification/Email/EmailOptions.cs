using ClassifiedAds.Infrastructure.Notification.Email.SendGrid;
using ClassifiedAds.Infrastructure.Notification.Email.SmtpClient;

namespace ClassifiedAds.Infrastructure.Notification.Email;

public class EmailOptions
{
    public string Provider { get; set; }

    public SmtpClientOptions SmtpClient { get; set; }

    public SendGridOptions SendGrid { get; set; }

    public bool UsedFake()
    {
        return Provider == "Fake";
    }

    public bool UsedSmtpClient()
    {
        return Provider == "SmtpClient";
    }

    public bool UsedSendGrid()
    {
        return Provider == "SendGrid";
    }
}
