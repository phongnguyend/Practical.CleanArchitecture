using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.WebAPI.Models.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI;

public class FilesControllerTests : TestBase
{
    public FilesControllerTests()
        : base()
    {
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    private async Task<List<FileEntryModel>> GetFilesAsync()
    {
        var files = await GetAsync<List<FileEntryModel>>("api/files");
        return files;
    }

    private async Task<FileEntryModel> GetFileByIdAsync(Guid id)
    {
        var file = await GetAsync<FileEntryModel>($"api/files/{id}");
        return file;
    }

    private async Task<FileEntryModel> UploadFileAsync(FileEntryModel file)
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

        var createdFile = await response.Content.ReadAs<FileEntryModel>();
        return createdFile;
    }

    private async Task<FileEntryModel> UpdateFileAsync(Guid id, FileEntryModel file)
    {
        var updatedProduct = await PutAsync<FileEntryModel>($"api/files/{id}", file);
        return updatedProduct;
    }

    private async Task DownloadFileAsync(FileEntryModel file)
    {
        var path = Path.Combine(AppSettings.DownloadsFolder, "Practical.CleanArchitecture", file.Id.ToString());
        Directory.CreateDirectory(path);

        using var response = await _httpClient.GetAsync($"api/files/{file.Id}/download");
        using var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.CreateNew);
        await response.Content.CopyToAsync(fileStream);
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
        var file = new FileEntryModel
        {
            Name = "Test",
            Description = "Description",
            FileName = "IntegrationTest.txt",
            Encrypted = true,
        };
        FileEntryModel createdFile = await UploadFileAsync(file);
        Assert.True(file.Id != createdFile.Id);
        Assert.Equal(file.Name, createdFile.Name);
        Assert.Equal(file.Description, createdFile.Description);

        var auditLogs = await GetAuditLogsAsync(createdFile.Id);
        Assert.Equal(3, auditLogs.Count);
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
        Assert.Equal(4, auditLogs.Count);
        Assert.Single(auditLogs, x => x.Action == "CREATED");
        Assert.Equal(3, auditLogs.Count(x => x.Action == "UPDATED"));

        // DOWNLOAD
        await DownloadFileAsync(createdFile);
        var path = Path.Combine(AppSettings.DownloadsFolder, "Practical.CleanArchitecture", createdFile.Id.ToString(), createdFile.FileName);
        Assert.True(File.Exists(path));

        // DELETE
        await DeleteFile(createdFile.Id);
        Assert.Null(await GetFileByIdAsync(createdFile.Id));
    }
}
