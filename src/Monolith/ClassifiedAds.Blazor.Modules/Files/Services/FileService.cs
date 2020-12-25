using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Services
{
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

        public string GetUploadUrl()
        {
            return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/files";
        }

        public async Task<List<FileEntryModel>> GetFiles()
        {
            var files = await GetAsync<List<FileEntryModel>>("api/files");
            return files;
        }

        public async Task<FileEntryModel> GetFileById(Guid id)
        {
            var file = await GetAsync<FileEntryModel>($"api/files/{id}");
            return file;
        }

        public async Task<FileEntryModel> UploadFile(FileEntryModel fileEntryModel, byte[] bytes)
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

        public async Task<FileEntryModel> UpdateFile(Guid id, FileEntryModel file)
        {
            var updatedFile = await PutAsync<FileEntryModel>($"api/files/{id}", file);
            return updatedFile;
        }

        public async Task DeleteFile(Guid id)
        {
            await DeleteAsync($"api/files/{id}");
        }

        public async Task<List<FileEntryAuditLogModel>> GetAuditLogs(Guid id)
        {
            var auditLogs = await GetAsync<List<FileEntryAuditLogModel>>($"api/files/{id}/auditlogs");
            return auditLogs;
        }
    }
}
