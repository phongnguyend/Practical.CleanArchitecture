using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Infrastructure.Notification.Sms;
using Microsoft.Extensions.Configuration;
using static ClassifiedAds.Services.Notification.Grpc.Sms;

namespace ClassifiedAds.Services.Identity.Commands.SmsMessages
{
    public class AddSmsMessageCommand : ICommand
    {
        public SmsMessageDTO SmsMessage { get; set; }
    }

    public class AddSmsMessageCommandHandler : ICommandHandler<AddSmsMessageCommand>
    {
        private readonly IConfiguration _configuration;

        public AddSmsMessageCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Handle(AddSmsMessageCommand command)
        {
            var client = new SmsClient(ChannelFactory.Create(_configuration["Services:Notification:Grpc"]));
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
