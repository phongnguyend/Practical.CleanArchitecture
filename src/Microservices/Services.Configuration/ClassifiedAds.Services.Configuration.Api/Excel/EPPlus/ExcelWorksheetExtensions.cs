using OfficeOpenXml;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Services.Configuration.Excel.EPPlus;

public static class ExcelWorksheetExtensions
{
    public static string GetCellValue(this ExcelWorksheet worksheet, string col, int row)
    {
        return worksheet.Cells[col + row].Value?.ToString()?.Trim();
    }

    public static string GetCellText(this ExcelWorksheet worksheet, string col, int row)
    {
        return worksheet.Cells[col + row].Text?.Trim();
    }

    public static string GetCellComment(this ExcelWorksheet worksheet, string col, int row)
    {
        return worksheet.Cells[col + row].Comment?.Text;
    }

    public static DateTime? GetCellDateTimeValue(this ExcelWorksheet worksheet, string col, int row)
    {
        double resFrom;
        DateTime dateTime;
        if (!string.IsNullOrWhiteSpace(worksheet.GetCellValue(col, row)))
        {
            if (double.TryParse(worksheet.GetCellValue(col, row), out resFrom))
            {
                return DateTime.FromOADate(resFrom);
            }

            if (DateTime.TryParse(worksheet.GetCellValue(col, row), out dateTime))
            {
                return dateTime;
            }
        }

        return null;
    }

    public static string VerifyHeader(this ExcelWorksheet worksheet, int headerIndex, Dictionary<string, string> expectedValues)
    {
        foreach (var correctHeader in expectedValues)
        {
            var currentHeader = worksheet.GetCellValue(correctHeader.Key, headerIndex);

            if (!correctHeader.Value.Equals(currentHeader, StringComparison.OrdinalIgnoreCase))
            {
                return $"Wrong Template! The expected value of cell [{correctHeader.Key}{headerIndex}] is: {correctHeader.Value} but the actual value is: {currentHeader}";
            }
        }

        return string.Empty;
    }
}
