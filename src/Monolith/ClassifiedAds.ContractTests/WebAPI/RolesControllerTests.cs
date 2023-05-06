using ApprovalTests;
using ApprovalTests.Reporters;
using ClassifiedAds.WebAPI.Models.Roles;
using NJsonSchema;
using Xunit;

namespace ClassifiedAds.ContractTests.WebAPI;

[UseReporter(typeof(VisualStudioReporter))]
public class RolesControllerTests
{
    [Fact]
    public void RoleModelTest()
    {
        var schema = JsonSchema.FromType<RoleModel>().ToJson();
        Approvals.Verify(schema);
    }
}
