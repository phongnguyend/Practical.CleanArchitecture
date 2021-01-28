using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Storage.DTOs;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using System.Threading.Tasks;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Storage.Commands
{
    public class AddAuditLogEntryCommand : ICommand
    {
        public AuditLogEntryDTO AuditLogEntry { get; set; }
    }

    public class AddAuditLogEntryCommandHandler : ICommandHandler<AddAuditLogEntryCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddAuditLogEntryCommandHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(AddAuditLogEntryCommand command, CancellationToken cancellationToken = default)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" },
            };

            var client = new AuditLogClient(ChannelFactory.Create(_configuration["Services:AuditLog:Grpc"]));
            client.AddAuditLogEntry(new AuditLog.Grpc.AddAuditLogEntryRequest
            {
                Entry = new AuditLog.Grpc.AuditLogEntryMessage
                {
                    ObjectId = command.AuditLogEntry.ObjectId,
                    UserId = command.AuditLogEntry.UserId.ToString(),
                    Action = command.AuditLogEntry.Action,
                    Log = command.AuditLogEntry.Log,
                    CreatedDateTime = Timestamp.FromDateTimeOffset(command.AuditLogEntry.CreatedDateTime),
                },
            }, headers);
        }
    }
}
