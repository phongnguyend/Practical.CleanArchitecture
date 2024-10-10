using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Csv;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Csv;

public class ExportProductsToCsvHandler : ICsvWriter<ExportProductsToCsv>
{
    public Task WriteAsync(ExportProductsToCsv data, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data.Products);

        return Task.CompletedTask;
    }
}
