using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
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

        private async Task<List<FileEntry>> GetFiles()
        {
            var files = await GetAsync<List<FileEntry>>("api/files");
            return files;
        }

        private async Task<FileEntry> GetFileById(Guid id)
        {
            var file = await GetAsync<FileEntry>($"api/files/{id}");
            return file;
        }

        private async Task<FileEntry> UploadFile(FileEntry file)
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

        private async Task<FileEntry> UpdateFile(Guid id, FileEntry file)
        {
            var updatedProduct = await PutAsync<FileEntry>($"api/files/{id}", file);
            return updatedProduct;
        }

        private async Task DeleteFile(Guid id)
        {
            await DeleteAsync($"api/files/{id}");
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
            FileEntry createdFile = await UploadFile(file);
            Assert.True(file.Id != createdFile.Id);
            Assert.Equal(file.Name, createdFile.Name);
            Assert.Equal(file.Description, createdFile.Description);

            // GET
            var files = await GetFiles();
            Assert.True(files.Count > 0);

            // GET ONE
            var refreshedFile = await GetFileById(createdFile.Id);
            Assert.Equal(refreshedFile.Id, createdFile.Id);
            Assert.Equal(refreshedFile.Name, createdFile.Name);
            Assert.Equal(refreshedFile.Description, createdFile.Description);

            // PUT
            refreshedFile.Name = "Test 2";
            var updatedProduct = await UpdateFile(refreshedFile.Id, refreshedFile);
            Assert.Equal(refreshedFile.Id, updatedProduct.Id);
            Assert.Equal("Test 2", updatedProduct.Name);
            Assert.Equal(refreshedFile.Description, updatedProduct.Description);

            // DELETE
            await DeleteFile(createdFile.Id);
            Assert.Null(await GetFileById(createdFile.Id));
        }
    }
}
