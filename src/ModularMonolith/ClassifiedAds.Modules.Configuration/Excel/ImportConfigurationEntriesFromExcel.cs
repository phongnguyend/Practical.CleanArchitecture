using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Modules.Configuration.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Configuration.Excel;

public class ImportConfigurationEntriesFromExcel : IExcelResponse
{
    public List<ConfigurationEntry> ConfigurationEntries { get; set; }
}
