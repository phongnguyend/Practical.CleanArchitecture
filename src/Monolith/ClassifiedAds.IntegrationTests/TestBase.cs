using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.IntegrationTests.Configuration;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.IntegrationTests;

public class TestBase
{
    public AppSettings AppSettings { get; set; } = new AppSettings();

    protected HttpClient _httpClient = new HttpClient();

    public TestBase()
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        configuration.Bind(AppSettings);

        _httpClient.BaseAddress = new Uri(AppSettings.WebAPI.Endpoint);
    }

    protected async Task GetTokenAsync()
    {
        var metaDataResponse = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = AppSettings.OpenIdConnect.Authority,
            Policy = { RequireHttps = AppSettings.OpenIdConnect.RequireHttpsMetadata },
        });

        var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = metaDataResponse.TokenEndpoint,
            ClientId = AppSettings.OpenIdConnect.ClientId,
            ClientSecret = AppSettings.OpenIdConnect.ClientSecret,
            UserName = AppSettings.Login.UserName,
            Password = AppSettings.Login.Password,
            Scope = AppSettings.Login.Scope,
        });

        var token = tokenResponse.AccessToken;
        _httpClient.UseBearerToken(token);
    }

    protected async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAs<T>();
        return data;
    }

    protected async Task<T> PostAsync<T>(string url, object data = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (data != null)
        {
            request.Content = data.AsJsonContent();
        }

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var createdObject = await response.Content.ReadAs<T>();
        return createdObject;
    }

    public async Task<T> PutAsync<T>(string url, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = data.AsJsonContent();

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var updatedObject = await response.Content.ReadAs<T>();
        return updatedObject;
    }

    protected async Task DeleteAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    }
}
