using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Pdf;

public class HtmlToPdfClient
{
    public const int MaxHtmlFileSize = 10 * 1024 * 1024;

    private readonly HttpClient _httpClient;

    public HtmlToPdfClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<byte[]> ConvertAsync(byte[] htmlFile, string fileName = "document.html", CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(htmlFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (htmlFile.Length == 0 || htmlFile.Length > MaxHtmlFileSize)
        {
            throw new ArgumentException("The HTML file must contain between 1 byte and 10 MB.", nameof(htmlFile));
        }

        var extension = Path.GetExtension(fileName);
        if (!string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The file must have an .html or .htm extension.", nameof(fileName));
        }

        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(htmlFile);
        file.Headers.ContentType = new MediaTypeHeaderValue("text/html") { CharSet = "utf-8" };
        form.Add(file, "file", Path.GetFileName(fileName));

        using var response = await _httpClient.PostAsync("convert-file", form, cancellationToken);
        response.EnsureSuccessStatusCode();
        if (!string.Equals(response.Content.Headers.ContentType?.MediaType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The HTML-to-PDF server did not return application/pdf.");
        }

        var pdf = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        if (!pdf.AsSpan().StartsWith("%PDF-"u8))
        {
            throw new InvalidDataException("The HTML-to-PDF server returned an invalid PDF document.");
        }

        return pdf;
    }
}
