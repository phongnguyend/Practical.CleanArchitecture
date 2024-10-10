using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Html;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.PdfConverters.DinkToPdf;

public class ExportProductsToPdfHandler : IPdfWriter<ExportProductsToPdf>
{
    private readonly IConverter _converter;
    private readonly IHtmlWriter<ExportProductsToHtml> _htmlWriter;

    public ExportProductsToPdfHandler(IConverter converter, IHtmlWriter<ExportProductsToHtml> htmlWriter)
    {
        _converter = converter;
        _htmlWriter = htmlWriter;
    }

    public async Task<byte[]> GetBytesAsync(ExportProductsToPdf data)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings =
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings() { Top = 10, Bottom = 15, Left = 10, Right = 10 },
            },
            Objects =
            {
                new ObjectSettings()
                {
                    PagesCount = true,
                    HtmlContent = await _htmlWriter.GetStringAsync(new ExportProductsToHtml {Products = data.Products}),
                    WebSettings = { DefaultEncoding = "utf-8", Background = true },
                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                },
            },
        };

        var bytes = _converter.Convert(doc);
        return bytes;
    }

    public async Task WriteAsync(ExportProductsToPdf data, Stream stream)
    {
        using var sw = new BinaryWriter(stream);
        sw.Write(await GetBytesAsync(data));
    }
}
