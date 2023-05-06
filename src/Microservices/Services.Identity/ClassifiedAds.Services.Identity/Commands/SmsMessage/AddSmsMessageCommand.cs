using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Identity.DTOs;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using System.Threading.Tasks;
using static ClassifiedAds.Services.Notification.Grpc.Sms;

namespace ClassifiedAds.Services.Identity.Commands.SmsMessages;

public class AddSmsMessageCommand : ICommand
{
    public SmsMessageDTO SmsMessage { get; set; }
}

public class AddSmsMessageCommandHandler : ICommandHandler<AddSmsMessageCommand>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddSmsMessageCommandHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(AddSmsMessageCommand command, CancellationToken cancellationToken = default)
    {
        var token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        var headers = new Metadata
        {
            { "Authorization", $"Bearer {token}" },
        };

        var client = new SmsClient(ChannelFactory.Create(_configuration["Services:Notification:Grpc"]));
        client.AddSmsMessage(new Notification.Grpc.AddSmsMessageRequest
        {
            Message = new Notification.Grpc.SmsMessage
            {
                Message = command.SmsMessage.Message,
                PhoneNumber = command.SmsMessage.PhoneNumber,
            },
        }, headers);
    }
}
