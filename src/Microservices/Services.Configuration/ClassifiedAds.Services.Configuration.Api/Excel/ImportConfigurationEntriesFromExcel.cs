using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Services.Configuration.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Services.Configuration.Excel;

public class ImportConfigurationEntriesFromExcel : IExcelResponse
{
    public List<ConfigurationEntry> ConfigurationEntries { get; set; }
}
