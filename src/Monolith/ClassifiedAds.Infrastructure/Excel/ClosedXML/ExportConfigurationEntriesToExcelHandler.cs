using ClassifiedAds.Application.ConfigurationEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClosedXML.Excel;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Excel.ClosedXML;

public class ExportConfigurationEntriesToExcelHandler : IExcelWriter<ExportConfigurationEntriesToExcel>
{
    public Task WriteAsync(ExportConfigurationEntriesToExcel data, Stream stream)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        worksheet.Cell("A1").Value = "Key";
        worksheet.Cell("B1").Value = "Value";
        worksheet.Range("A1:B1").Style.Font.Bold = true;

        int i = 2;
        foreach (var row in data.ConfigurationEntries)
        {
            worksheet.Cell("A" + i).Value = row.Key;
            worksheet.Cell("B" + i).Value = row.Value;
            i++;
        }

        workbook.SaveAs(stream);

        return Task.CompletedTask;
    }
}
