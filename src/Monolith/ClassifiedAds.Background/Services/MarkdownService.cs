using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.Services;

public class MarkdownService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    private readonly IConfiguration _configuration;

    public MarkdownService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> ConvertToMarkdownAsync(byte[] content,string fileName, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        form.Add(fileContent, "formFile", fileName);
        form.Add(new StringContent("Test Name"), "name");

        var response = await _httpClient.PostAsync(_configuration["TextExtracting:MarkItDownServer:Endpoint"], form, cancellationToken);
        response.EnsureSuccessStatusCode();

        var markdown = await response.Content.ReadAsStringAsync(cancellationToken);

        return markdown;
    }
}
