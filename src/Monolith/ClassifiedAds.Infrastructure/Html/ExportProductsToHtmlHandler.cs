using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Html;
using RazorLight;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Html;

public class ExportProductsToHtmlHandler : IHtmlWriter<ExportProductsToHtml>
{
    private readonly IRazorLightEngine _razorLightEngine;

    public ExportProductsToHtmlHandler(IRazorLightEngine razorLightEngine)
    {
        _razorLightEngine = razorLightEngine;
    }

    public async Task WriteAsync(ExportProductsToHtml data, Stream stream)
    {
        using var sw = new StreamWriter(stream, Encoding.UTF8);
        await sw.WriteAsync(await GetStringAsync(data));
    }

    public async Task<string> GetStringAsync(ExportProductsToHtml data)
    {
        var template = Path.Combine(Environment.CurrentDirectory, $"Templates/ProductList.cshtml");
        string html = await _razorLightEngine.CompileRenderAsync(template, data.Products);
        return html;
    }
}
