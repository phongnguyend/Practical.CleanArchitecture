using ClassifiedAds.CrossCuttingConcerns.PdfConverter;
using PuppeteerSharp;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.PdfConverters.PuppeteerSharp
{
    public class PuppeteerSharpConverter : IPdfConverter
    {
        public Stream Convert(string html, CrossCuttingConcerns.PdfConverter.PdfOptions pdfOptions = null)
        {
            return ConvertAsync(html, pdfOptions).GetAwaiter().GetResult();
        }

        public async Task<Stream> ConvertAsync(string html, CrossCuttingConcerns.PdfConverter.PdfOptions pdfOptions = null)
        {
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            return new MemoryStream(await page.PdfDataAsync(new global::PuppeteerSharp.PdfOptions
            {
                PrintBackground = true,
            }));
        }
    }
}
