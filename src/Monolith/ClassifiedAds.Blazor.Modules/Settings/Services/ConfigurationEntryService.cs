using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Settings.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Settings.Services;

public class ConfigurationEntryService : HttpService
{
    public ConfigurationEntryService(HttpClient httpClient, ITokenManager tokenManager)
        : base(httpClient, tokenManager)
    {
    }

    public async Task<List<ConfigurationEntryModel>> GetListAsync()
    {
        var list = await GetAsync<List<ConfigurationEntryModel>>("api/ConfigurationEntries");
        return list;
    }

    public async Task<ConfigurationEntryModel> GetByIdAsync(Guid id)
    {
        var entry = await GetAsync<ConfigurationEntryModel>($"api/ConfigurationEntries/{id}");
        return entry;
    }

    public async Task<ConfigurationEntryModel> CreateAsync(ConfigurationEntryModel entry)
    {
        var createdEntry = await PostAsync<ConfigurationEntryModel>($"api/ConfigurationEntries", entry);
        return createdEntry;
    }

    public async Task<ConfigurationEntryModel> UpdateAsync(ConfigurationEntryModel entry)
    {
        var updatedEntry = await PutAsync<ConfigurationEntryModel>($"api/ConfigurationEntries/{entry.Id}", entry);
        return updatedEntry;
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"api/ConfigurationEntries/{id}");
    }

    public async Task<List<FileEntryAuditLogModel>> GetAuditLogsAsync(Guid id)
    {
        var auditLogs = await GetAsync<List<FileEntryAuditLogModel>>($"api/ConfigurationEntries/{id}/AuditLogs");
        return auditLogs;
    }

    public string GetExportExcelUrl()
    {
        return $"{_httpClient.BaseAddress.AbsoluteUri.Trim('/')}/api/ConfigurationEntries/ExportAsExcel";
    }
}
