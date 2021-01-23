using ApprovalTests;
using ApprovalTests.Reporters;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.WebAPI.Controllers;
using ClassifiedAds.WebAPI.Models.Files;
using NJsonSchema;
using Xunit;

namespace ClassifiedAds.ContractTests.WebAPI
{
    [UseReporter(typeof(VisualStudioReporter))]
    public class FilesControllerTests
    {
        [Fact]
        public void FileEntryModelTest()
        {
            var schema = JsonSchema.FromType<FileEntryModel>().ToJson();
            Approvals.Verify(schema);
        }

        [Fact]
        public void UploadFileTest()
        {
            var schema = JsonSchema.FromType<UploadFile>().ToJson();
            Approvals.Verify(schema);
        }

        [Fact]
        public void AuditLogEntryDTOTest()
        {
            var schema = JsonSchema.FromType<AuditLogEntryDTO>().ToJson();
            Approvals.Verify(schema);
        }
    }
}
