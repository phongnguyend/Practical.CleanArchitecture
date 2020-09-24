using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Notification.Email;

namespace ClassifiedAds.Services.Identity.Api.Commands.EmailMessages
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
            // TODO: xxx
        }
    }
}
