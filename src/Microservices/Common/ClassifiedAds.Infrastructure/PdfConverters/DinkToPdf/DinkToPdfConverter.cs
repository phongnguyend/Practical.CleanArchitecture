using ClassifiedAds.CrossCuttingConcerns.PdfConverter;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.PdfConverters.DinkToPdf
{
    public class DinkToPdfConverter : IPdfConverter
    {
        private readonly IConverter _converter;

        public DinkToPdfConverter(IConverter converter)
        {
            _converter = converter;
        }

        public Stream Convert(string html, PdfOptions pdfOptions = null)
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
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8", Background = true },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
                    },
                },
            };

            byte[] pdf = _converter.Convert(doc);

            return new MemoryStream(pdf);
        }

        public Task<Stream> ConvertAsync(string html, PdfOptions pdfOptions = null)
        {
            return Task.FromResult(Convert(html, pdfOptions));
        }
    }
}
