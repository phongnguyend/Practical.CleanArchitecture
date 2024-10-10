using ClassifiedAds.Application.ConfigurationEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Excel.ClosedXML;

public class ImportConfigurationEntriesFromExcelHandler : IExcelReader<ImportConfigurationEntriesFromExcel>
{
    private static Dictionary<string, string> GetCorrectHeaders()
    {
        return new Dictionary<string, string>
        {
            { "A", "Key" },
            { "B", "Value" },
        };
    }

    public Task<ImportConfigurationEntriesFromExcel> ReadAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        string result = worksheet.VerifyHeader(1, GetCorrectHeaders());
        if (!string.IsNullOrEmpty(result))
        {
            throw new ValidationException(result);
        }

        var rows = new List<ConfigurationEntry>();

        for (var i = 2; i <= worksheet.LastRowUsed().RowNumber(); i++)
        {
            var row = new ConfigurationEntry
            {
                Key = worksheet.Cell("A" + i).GetString(),
                Value = worksheet.Cell("B" + i).GetString(),
            };

            rows.Add(row);
        }

        return Task.FromResult(new ImportConfigurationEntriesFromExcel { ConfigurationEntries = rows });
    }
}
