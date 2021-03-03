using ClassifiedAds.WebAPI.Models.ConfigurationEntries;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI
{
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

            // DELETE
            await DeleteAsync(created.Id);
            await Assert.ThrowsAsync<HttpRequestException>(async () => await GetByIdAsync(created.Id));
        }
    }
}
