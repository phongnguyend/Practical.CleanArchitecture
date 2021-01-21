using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI
{
    public class FilesControllerTests : TestBase
    {
        public FilesControllerTests()
            : base()
        {
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private async Task<List<FileEntry>> GetFilesAsync()
        {
            var files = await GetAsync<List<FileEntry>>("api/files");
            return files;
        }

        private async Task<FileEntry> GetFileByIdAsync(Guid id)
        {
            var file = await GetAsync<FileEntry>($"api/files/{id}");
            return file;
        }

        private async Task<FileEntry> UploadFileAsync(FileEntry file)
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Test"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "formFile", file.FileName);
            form.Add(new StringContent(file.Name), "name");
            form.Add(new StringContent(file.Description), "description");
            form.Add(new StringContent(file.Encrypted.ToString()), "encrypted");

            var response = await _httpClient.PostAsync($"api/files", form);
            response.EnsureSuccessStatusCode();

            var createdFile = await response.Content.ReadAs<FileEntry>();
            return createdFile;
        }

        private async Task<FileEntry> UpdateFileAsync(Guid id, FileEntry file)
        {
            var updatedProduct = await PutAsync<FileEntry>($"api/files/{id}", file);
            return updatedProduct;
        }

        private async Task DeleteFile(Guid id)
        {
            await DeleteAsync($"api/files/{id}");
        }

        public async Task<List<AuditLogEntryDTO>> GetAuditLogsAsync(Guid id)
        {
            var auditLogs = await GetAsync<List<AuditLogEntryDTO>>($"api/files/{id}/auditlogs");
            return auditLogs;
        }

        [Fact]
        public async Task AllInOne()
        {
            await GetTokenAsync();

            // POST
            var file = new FileEntry
            {
                Name = "Test",
                Description = "Description",
                FileName = "IntegrationTest.txt",
                Encrypted = true,
            };
            FileEntry createdFile = await UploadFileAsync(file);
            Assert.True(file.Id != createdFile.Id);
            Assert.Equal(file.Name, createdFile.Name);
            Assert.Equal(file.Description, createdFile.Description);

            var auditLogs = await GetAuditLogsAsync(createdFile.Id);
            Assert.Equal(2, auditLogs.Count);
            Assert.Contains(auditLogs, x => x.Action == "CREATED");
            Assert.Contains(auditLogs, x => x.Action == "UPDATED");

            // GET
            var files = await GetFilesAsync();
            Assert.True(files.Count > 0);

            // GET ONE
            var refreshedFile = await GetFileByIdAsync(createdFile.Id);
            Assert.Equal(refreshedFile.Id, createdFile.Id);
            Assert.Equal(refreshedFile.Name, createdFile.Name);
            Assert.Equal(refreshedFile.Description, createdFile.Description);

            // PUT
            refreshedFile.Name = "Test 2";
            var updatedProduct = await UpdateFileAsync(refreshedFile.Id, refreshedFile);
            Assert.Equal(refreshedFile.Id, updatedProduct.Id);
            Assert.Equal("Test 2", updatedProduct.Name);
            Assert.Equal(refreshedFile.Description, updatedProduct.Description);

            auditLogs = await GetAuditLogsAsync(createdFile.Id);
            Assert.Equal(3, auditLogs.Count);
            Assert.Single(auditLogs, x => x.Action == "CREATED");
            Assert.Equal(2, auditLogs.Count(x => x.Action == "UPDATED"));

            // DELETE
            await DeleteFile(createdFile.Id);
            Assert.Null(await GetFileByIdAsync(createdFile.Id));
        }
    }
}
