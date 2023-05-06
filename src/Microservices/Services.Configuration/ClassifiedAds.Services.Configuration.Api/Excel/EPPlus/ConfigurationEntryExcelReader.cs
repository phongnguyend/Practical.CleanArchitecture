using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Services.Configuration.Entities;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassifiedAds.Services.Configuration.Excel.EPPlus;

public class ConfigurationEntryExcelReader : IExcelReader<List<ConfigurationEntry>>
{
    public List<ConfigurationEntry> Read(Stream stream)
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

        return rows;
    }

    private static Dictionary<string, string> GetCorrectHeaders()
    {
        return new Dictionary<string, string>
        {
            { "A", "Key" },
            { "B", "Value" },
        };
    }
}
