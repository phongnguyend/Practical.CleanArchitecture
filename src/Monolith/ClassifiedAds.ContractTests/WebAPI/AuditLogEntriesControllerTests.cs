using ApprovalTests;
using ApprovalTests.Reporters;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using NJsonSchema;
using Xunit;

namespace ClassifiedAds.ContractTests.WebAPI;

[UseReporter(typeof(VisualStudioReporter))]
public class AuditLogEntriesControllerTests
{
    [Fact]
    public void AuditLogEntryDTOTest()
    {
        var schema = JsonSchema.FromType<AuditLogEntryDTO>().ToJson();
        Approvals.Verify(schema);
    }
}
