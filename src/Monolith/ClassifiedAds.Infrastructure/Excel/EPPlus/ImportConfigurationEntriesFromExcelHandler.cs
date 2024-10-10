using ClassifiedAds.Application.ConfigurationEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Excel.EPPlus;

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
        using var pck = new ExcelPackage(stream);
        var worksheet = pck.Workbook.Worksheets.First();

        string result = worksheet.VerifyHeader(1, GetCorrectHeaders());
        if (!string.IsNullOrEmpty(result))
        {
            throw new ValidationException(result);
        }

        var rows = new List<ConfigurationEntry>();

        for (var i = 2; i <= worksheet.Dimension.End.Row; i++)
        {
            var row = new ConfigurationEntry
            {
                Key = worksheet.GetCellValue("A", i),
                Value = worksheet.GetCellValue("B", i),
            };

            rows.Add(row);
        }

        return Task.FromResult(new ImportConfigurationEntriesFromExcel { ConfigurationEntries = rows });
    }
}
