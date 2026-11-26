using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Services;

public class FileService : HttpService
{
    public FileService(HttpClient httpClient, ITokenManager tokenManager)
        : base(httpClient, tokenManager)
    {
    }

    public string GetDownloadUrl(Guid id)
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/files/{id}/download";
    }

    public string GetDownloadTextUrl(Guid id)
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/files/{id}/downloadtext";
    }

    public string GetDownloadChunkUrl(Guid id, string chunkName)
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/files/{id}/downloadchunk/{chunkName}";
    }

    public string GetUploadUrl()
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/files";
    }

    public async Task<List<FileEntryModel>> GetFilesAsync()
    {
        var files = await GetAsync<List<FileEntryModel>>("api/files");
        return files;
    }

    public async Task<FileEntryModel> GetFileByIdAsync(Guid id)
    {
        var file = await GetAsync<FileEntryModel>($"api/files/{id}");
        return file;
    }

    public async Task<FileEntryModel> UploadFileAsync(FileEntryModel fileEntryModel, byte[] bytes)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        form.Add(fileContent, "formFile", fileEntryModel.FileName);
        form.Add(new StringContent(fileEntryModel.Name), "name");
        form.Add(new StringContent(fileEntryModel.Description), "description");
        form.Add(new StringContent(fileEntryModel.Encrypted.ToString()), "encrypted");

        await SetBearerToken();

        var response = await _httpClient.PostAsync($"api/files", form);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAs<FileEntryModel>();
        return result;
    }

    public async Task<FileEntryModel> UpdateFileAsync(Guid id, FileEntryModel file)
    {
        var updatedFile = await PutAsync<FileEntryModel>($"api/files/{id}", file);
        return updatedFile;
    }

    public async Task DeleteFileAsync(Guid id)
    {
        await DeleteAsync($"api/files/{id}");
    }

    public async Task<List<FileEntryAuditLogModel>> GetAuditLogsAsync(Guid id)
    {
        var auditLogs = await GetAsync<List<FileEntryAuditLogModel>>($"api/files/{id}/auditlogs");
        return auditLogs;
    }

    public async Task<List<FileEntryVectorSearchResultModel>> VectorSearchFilesAsync(string searchText)
    {
        var files = await GetAsync<List<FileEntryVectorSearchResultModel>>($"api/files/vectorsearch?searchText={searchText}");
        return files;
    }
}
