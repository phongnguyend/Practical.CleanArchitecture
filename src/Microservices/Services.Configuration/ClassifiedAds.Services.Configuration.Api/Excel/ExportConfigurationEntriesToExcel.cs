using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Services.Configuration.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Services.Configuration.Excel;

public class ExportConfigurationEntriesToExcel : IExcelRequest
{
    public List<ConfigurationEntry> ConfigurationEntries { get; set; }
}
