using ClassifiedAds.Application;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static ClassifiedAds.Services.Notification.Grpc.Sms;

namespace ClassifiedAds.Services.Notification.Grpc.Services;

[Authorize]
public class SmsMessageService : SmsBase
{
    private readonly ILogger<SmsMessageService> _logger;
    private readonly Dispatcher _dispatcher;

    public SmsMessageService(ILogger<SmsMessageService> logger, Dispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [AllowAnonymous]
    public override async Task<AddSmsMessageResponse> AddSmsMessage(AddSmsMessageRequest request, ServerCallContext context)
    {
        var message = new Entities.SmsMessage
        {
            Message = request.Message.Message,
            PhoneNumber = request.Message.PhoneNumber,
        };

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<Entities.SmsMessage>(message));

        var response = new AddSmsMessageResponse
        {
            Message = request.Message
        };

        response.Message.Id = message.Id.ToString();

        return response;
    }
}
