using ClassifiedAds.CrossCuttingConcerns.Csv;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Csv;

public class CsvReader<T> : ICsvReader<T>
{
    public IEnumerable<T> Read(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<T>().ToList();
    }
}
