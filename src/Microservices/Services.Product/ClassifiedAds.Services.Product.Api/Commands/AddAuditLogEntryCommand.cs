using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Product.DTOs;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Product.Commands
{
    public class AddAuditLogEntryCommand : ICommand
    {
        public AuditLogEntryDTO AuditLogEntry { get; set; }
    }

    public class AddAuditLogEntryCommandHandler : ICommandHandler<AddAuditLogEntryCommand>
    {
        private readonly IConfiguration _configuration;

        public AddAuditLogEntryCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Handle(AddAuditLogEntryCommand command)
        {
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
            });
        }
    }
}
