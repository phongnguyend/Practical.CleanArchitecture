using ClosedXML.Excel;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Excel.ClosedXML;

public static class IXLWorksheetExtensions
{
    public static string VerifyHeader(this IXLWorksheet worksheet, int headerIndex, Dictionary<string, string> expectedValues)
    {
        foreach (var correctHeader in expectedValues)
        {
            var currentHeader = worksheet.Cell(correctHeader.Key + headerIndex).GetString();

            if (!correctHeader.Value.Equals(currentHeader, StringComparison.OrdinalIgnoreCase))
            {
                return $"Wrong Template! The expected value of cell [{correctHeader.Key}{headerIndex}] is: {correctHeader.Value} but the actual value is: {currentHeader}";
            }
        }

        return string.Empty;
    }
}
