using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClassifiedAds.Infrastructure.Excel.ExcelDataReader;

public class ConfigurationEntryExcelReader : IExcelReader<List<ConfigurationEntry>>
{
    public List<ConfigurationEntry> Read(Stream stream)
    {
        var rows = new List<ConfigurationEntry>();
        int headerIndex = 0;

        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            int sheetIndex = 0;
            do
            {
                string sheetName = reader.Name;
                if (sheetName != "Sheet1")
                {
                    continue;
                }

                int rowIndex = 0;
                while (reader.Read())
                {
                    if (rowIndex < headerIndex)
                    {
                        rowIndex++;
                        continue;
                    }
                    else if (rowIndex == headerIndex)
                    {
                        foreach (var header in GetCorrectHeaders())
                        {
                            if (!string.Equals(reader.GetValue(header.Key)?.ToString(), header.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new ValidationException("Wrong Template!");
                            }
                        }

                        rowIndex++;
                        continue;
                    }

                    var row = new ConfigurationEntry
                    {
                        Key = reader.GetValue(0)?.ToString(),
                        Value = reader.GetValue(1)?.ToString(),
                    };

                    rows.Add(row);
                    rowIndex++;
                }

                sheetIndex++;
            }
            while (reader.NextResult());
        }

        return rows;
    }

    private static Dictionary<int, string> GetCorrectHeaders()
    {
        return new Dictionary<int, string>
        {
            { 0, "Key" },
            { 1, "Value" },
        };
    }
}
