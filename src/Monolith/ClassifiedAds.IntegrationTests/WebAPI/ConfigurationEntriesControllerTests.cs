using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.WebAPI.Models.ConfigurationEntries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI;

public class ConfigurationEntriesControllerTests : TestBase
{
    public ConfigurationEntriesControllerTests()
        : base()
    {
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    private async Task<List<ConfigurationEntryModel>> GetListAsync()
    {
        return await GetAsync<List<ConfigurationEntryModel>>("api/ConfigurationEntries");
    }

    private async Task<ConfigurationEntryModel> GetByIdAsync(Guid id)
    {
        return await GetAsync<ConfigurationEntryModel>($"api/ConfigurationEntries/{id}");
    }

    private async Task<ConfigurationEntryModel> CreateAsync(ConfigurationEntryModel model)
    {
        return await PostAsync<ConfigurationEntryModel>("api/ConfigurationEntries", model);
    }

    private async Task<ConfigurationEntryModel> UpdateAsync(Guid id, ConfigurationEntryModel model)
    {
        return await PutAsync<ConfigurationEntryModel>($"api/ConfigurationEntries/{id}", model);
    }

    private async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"api/ConfigurationEntries/{id}");
    }

    private async Task ExportAsExcelAsync(string path, string fileName)
    {
        using var response = await _httpClient.GetAsync($"api/ConfigurationEntries/ExportAsExcel");
        using var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.CreateNew);
        await response.Content.CopyToAsync(fileStream);
    }

    private async Task<List<ConfigurationEntryModel>> ImportExcelAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        form.Add(fileContent, "formFile", "ConfigurationEntries.xlsx");
        form.Add(new StringContent("ConfigurationEntries.xlsx"), "name");

        var response = await _httpClient.PostAsync($"api/ConfigurationEntries/ImportExcel", form);
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadAs<List<ConfigurationEntryModel>>();
        return products;
    }

    [Fact]
    public async Task AllInOne_NonSensiveValue()
    {
        await GetTokenAsync();

        // POST
        var model = new ConfigurationEntryModel
        {
            Key = $"KEY_{Guid.NewGuid()}",
            Value = "VALUE1",
            Description = "Description",
        };

        var created = await CreateAsync(model);
        Assert.True(model.Id != created.Id);
        Assert.Equal(model.Key, created.Key);
        Assert.Equal("VALUE1", created.Value);
        Assert.Equal(model.Description, created.Description);

        // GET
        var list = await GetListAsync();
        Assert.True(list.Count > 0);

        // GET ONE
        var refreshed = await GetByIdAsync(created.Id);
        Assert.Equal(refreshed.Id, created.Id);
        Assert.Equal(refreshed.Key, created.Key);
        Assert.Equal(refreshed.Value, created.Value);
        Assert.Equal(refreshed.Description, created.Description);

        // PUT
        refreshed.Value = "VALUE2";
        var updated = await UpdateAsync(refreshed.Id, refreshed);
        Assert.Equal(refreshed.Id, updated.Id);
        Assert.Equal("VALUE2", updated.Value);
        Assert.Equal(refreshed.Description, updated.Description);

        // PUT
        refreshed.Value = "VALUE3";
        refreshed.IsSensitive = true;
        updated = await UpdateAsync(refreshed.Id, refreshed);
        Assert.Equal(refreshed.Id, updated.Id);
        Assert.NotEqual("VALUE3", updated.Value);
        Assert.Equal(refreshed.Description, updated.Description);

        var path = Path.Combine(AppSettings.DownloadsFolder, "Practical.CleanArchitecture", Guid.NewGuid().ToString());
        Directory.CreateDirectory(path);

        // EXPORT Excel
        await ExportAsExcelAsync(path, "ConfigurationEntries.xlsx");
        Assert.True(File.Exists(Path.Combine(path, "ConfigurationEntries.xlsx")));

        // IMPORT Excel
        var importingEntries = await ImportExcelAsync(Path.Combine(path, "ConfigurationEntries.xlsx"));
        Assert.True(importingEntries.Count > 0);

        // DELETE
        await DeleteAsync(created.Id);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task AllInOne_SensiveValue()
    {
        await GetTokenAsync();

        // POST
        var model = new ConfigurationEntryModel
        {
            Key = $"KEY_{Guid.NewGuid()}",
            Value = "VALUE1",
            Description = "Description",
            IsSensitive = true,
        };

        var created = await CreateAsync(model);
        Assert.True(model.Id != created.Id);
        Assert.Equal(model.Key, created.Key);
        Assert.NotEqual("VALUE1", created.Value);
        Assert.Equal(model.Description, created.Description);

        // GET
        var list = await GetListAsync();
        Assert.True(list.Count > 0);

        // GET ONE
        var refreshed = await GetByIdAsync(created.Id);
        Assert.Equal(refreshed.Id, created.Id);
        Assert.Equal(refreshed.Key, created.Key);
        Assert.Equal(refreshed.Value, created.Value);
        Assert.Equal(refreshed.Description, created.Description);

        // PUT
        refreshed.Value = "VALUE2";
        var updated = await UpdateAsync(refreshed.Id, refreshed);
        Assert.Equal(refreshed.Id, updated.Id);
        Assert.NotEqual("VALUE2", updated.Value);
        Assert.Equal(refreshed.Description, updated.Description);

        // PUT
        refreshed.Value = "VALUE3";
        refreshed.IsSensitive = false;
        updated = await UpdateAsync(refreshed.Id, refreshed);
        Assert.Equal(refreshed.Id, updated.Id);
        Assert.Equal("VALUE3", updated.Value);
        Assert.Equal(refreshed.Description, updated.Description);

        var path = Path.Combine(AppSettings.DownloadsFolder, "Practical.CleanArchitecture", Guid.NewGuid().ToString());
        Directory.CreateDirectory(path);

        // EXPORT Excel
        await ExportAsExcelAsync(path, "ConfigurationEntries.xlsx");
        Assert.True(File.Exists(Path.Combine(path, "ConfigurationEntries.xlsx")));

        // IMPORT Excel
        var importingEntries = await ImportExcelAsync(Path.Combine(path, "ConfigurationEntries.xlsx"));
        Assert.True(importingEntries.Count > 0);

        // DELETE
        await DeleteAsync(created.Id);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await GetByIdAsync(created.Id));
    }
}
