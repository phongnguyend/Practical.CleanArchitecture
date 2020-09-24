using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Storage.DTOs;
using Google.Protobuf.WellKnownTypes;
using static ClassifiedAds.Services.AuditLog.Grpc.AuditLog;

namespace ClassifiedAds.Services.Storage.Commands
{
    public class AddAuditLogEntryCommand : ICommand
    {
        public AuditLogEntryDTO AuditLogEntry { get; set; }
    }

    public class AddAuditLogEntryCommandHandler : ICommandHandler<AddAuditLogEntryCommand>
    {
        public AddAuditLogEntryCommandHandler()
        {
        }

        public void Handle(AddAuditLogEntryCommand command)
        {
            var client = new AuditLogClient(ChannelFactory.Create("https://localhost:5002"));
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
