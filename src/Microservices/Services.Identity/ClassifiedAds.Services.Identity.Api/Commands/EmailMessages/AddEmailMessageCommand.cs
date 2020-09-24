using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Infrastructure.Notification.Email;
using static ClassifiedAds.Services.Notification.Grpc.Email;

namespace ClassifiedAds.Services.Identity.Commands.EmailMessages
{
    public class AddEmailMessageCommand : ICommand
    {
        public EmailMessageDTO EmailMessage { get; set; }
    }

    public class AddEmailMessageCommandHandler : ICommandHandler<AddEmailMessageCommand>
    {
        public AddEmailMessageCommandHandler()
        {
        }

        public void Handle(AddEmailMessageCommand command)
        {
            var client = new EmailClient(ChannelFactory.Create("https://localhost:5003"));
            client.AddEmailMessage(new Notification.Grpc.AddEmailMessageRequest
            {
                Message = new Notification.Grpc.EmailMessage
                {
                    From = command.EmailMessage.From,
                    Tos = command.EmailMessage.Tos,
                    CCs = command.EmailMessage.CCs ?? string.Empty,
                    BCCs = command.EmailMessage.BCCs ?? string.Empty,
                    Subject = command.EmailMessage.Subject,
                    Body = command.EmailMessage.Body,
                },
            });
        }
    }
}
