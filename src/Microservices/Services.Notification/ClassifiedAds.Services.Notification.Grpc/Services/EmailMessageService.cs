using ClassifiedAds.Application;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static ClassifiedAds.Services.Notification.Grpc.Email;

namespace ClassifiedAds.Services.Notification.Grpc.Services
{
    public class EmailMessageService : EmailBase
    {
        private readonly ILogger<EmailMessageService> _logger;
        private readonly Dispatcher _dispatcher;

        public EmailMessageService(ILogger<EmailMessageService> logger, Dispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public override Task<AddEmailMessageResponse> AddEmailMessage(AddEmailMessageRequest request, ServerCallContext context)
        {
            var message = new Entities.EmailMessage
            {
                From = request.Message.From,
                Tos = request.Message.Tos,
                CCs = request.Message.CCs,
                BCCs = request.Message.BCCs,
                Subject = request.Message.Subject,
                Body = request.Message.Body,
            };

            _dispatcher.Dispatch(new AddOrUpdateEntityCommand<Entities.EmailMessage>(message));

            var response = new AddEmailMessageResponse
            {
                Message = request.Message
            };

            response.Message.Id = message.Id.ToString();

            return Task.FromResult(response);
        }
    }
}
