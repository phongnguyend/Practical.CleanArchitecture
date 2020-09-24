using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Infrastructure.Notification.Sms;
using static ClassifiedAds.Services.Notification.Grpc.Sms;

namespace ClassifiedAds.Services.Identity.Commands.SmsMessages
{
    public class AddSmsMessageCommand : ICommand
    {
        public SmsMessageDTO SmsMessage { get; set; }
    }

    public class AddSmsMessageCommandHandler : ICommandHandler<AddSmsMessageCommand>
    {
        public AddSmsMessageCommandHandler()
        {
        }

        public void Handle(AddSmsMessageCommand command)
        {
            var client = new SmsClient(ChannelFactory.Create("https://localhost:5003"));
            client.AddSmsMessage(new Notification.Grpc.AddSmsMessageRequest
            {
                Message = new Notification.Grpc.SmsMessage
                {
                    Message = command.SmsMessage.Message,
                    PhoneNumber = command.SmsMessage.PhoneNumber,
                },
            });
        }
    }
}
