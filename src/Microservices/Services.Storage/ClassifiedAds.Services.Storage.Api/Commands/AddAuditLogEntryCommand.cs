using ClassifiedAds.Application;
using ClassifiedAds.Services.AuditLog.Contracts.DTOs;

namespace ClassifiedAds.Services.Product.Api.Commands
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
            // TODO: xxx
        }
    }
}
