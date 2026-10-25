using ClassifiedAds.CrossCuttingConcerns.Html;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Infrastructure.Pdf;
using ClassifiedAds.Modules.Product.Html;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.Pdf;

public class ExportProductsToPdfHandler : IPdfWriter<ExportProductsToPdf>
{
    private readonly HtmlToPdfClient _client;
    private readonly IHtmlWriter<ExportProductsToHtml> _htmlWriter;

    public ExportProductsToPdfHandler(HtmlToPdfClient client, IHtmlWriter<ExportProductsToHtml> htmlWriter)
    {
        _client = client;
        _htmlWriter = htmlWriter;
    }

    public async Task<byte[]> GetBytesAsync(ExportProductsToPdf data)
    {
        var html = await _htmlWriter.GetStringAsync(new ExportProductsToHtml { Products = data.Products });
        return await _client.ConvertAsync(Encoding.UTF8.GetBytes(html), "products.html");
    }

    public async Task WriteAsync(ExportProductsToPdf data, Stream stream)
    {
        var pdf = await GetBytesAsync(data);
        await stream.WriteAsync(pdf);
    }
}
