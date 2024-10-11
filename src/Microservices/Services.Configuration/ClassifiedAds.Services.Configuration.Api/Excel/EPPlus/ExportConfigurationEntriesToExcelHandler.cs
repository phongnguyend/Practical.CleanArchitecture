using ClassifiedAds.CrossCuttingConcerns.Excel;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Configuration.Excel.EPPlus;

public class ExportConfigurationEntriesToExcelHandler : IExcelWriter<ExportConfigurationEntriesToExcel>
{
    public Task WriteAsync(ExportConfigurationEntriesToExcel data, Stream stream)
    {
        using var pck = new ExcelPackage();
        var worksheet = pck.Workbook.Worksheets.Add("Sheet1");

        worksheet.Cells["A1"].Value = "Key";
        worksheet.Cells["B1"].Value = "Value";
        worksheet.Cells["A1:B1"].Style.Font.Bold = true;

        int i = 2;
        foreach (var row in data.ConfigurationEntries)
        {
            worksheet.Cells["A" + i].Value = row.Key;
            worksheet.Cells["B" + i].Value = row.Value;
            i++;
        }

        pck.SaveAs(stream);

        return Task.CompletedTask;
    }
}
