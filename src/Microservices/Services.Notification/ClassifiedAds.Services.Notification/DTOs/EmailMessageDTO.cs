using ClassifiedAds.Infrastructure.Notification.Email;

namespace ClassifiedAds.Services.Notification.DTOs;

public class EmailMessageDTO : IEmailMessage
{
    public string From { get; set; }

    public string Tos { get; set; }

    public string CCs { get; set; }

    public string BCCs { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
}
