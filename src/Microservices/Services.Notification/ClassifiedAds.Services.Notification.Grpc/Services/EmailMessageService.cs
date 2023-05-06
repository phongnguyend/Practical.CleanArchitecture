using ClassifiedAds.Application;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static ClassifiedAds.Services.Notification.Grpc.Email;

namespace ClassifiedAds.Services.Notification.Grpc.Services;

[Authorize]
public class EmailMessageService : EmailBase
{
    private readonly ILogger<EmailMessageService> _logger;
    private readonly Dispatcher _dispatcher;

    public EmailMessageService(ILogger<EmailMessageService> logger, Dispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [AllowAnonymous]
    public override async Task<AddEmailMessageResponse> AddEmailMessage(AddEmailMessageRequest request, ServerCallContext context)
    {
        /// var user = context.GetHttpContext().User;

        var message = new Entities.EmailMessage
        {
            From = request.Message.From,
            Tos = request.Message.Tos,
            CCs = request.Message.CCs,
            BCCs = request.Message.BCCs,
            Subject = request.Message.Subject,
            Body = request.Message.Body,
        };

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<Entities.EmailMessage>(message));

        var response = new AddEmailMessageResponse
        {
            Message = request.Message
        };

        response.Message.Id = message.Id.ToString();

        return response;
    }
}
