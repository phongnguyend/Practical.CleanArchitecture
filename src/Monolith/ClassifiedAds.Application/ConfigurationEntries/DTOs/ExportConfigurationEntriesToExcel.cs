using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Application.ConfigurationEntries.DTOs;

public class ExportConfigurationEntriesToExcel : IExcelRequest
{
    public List<ConfigurationEntry> ConfigurationEntries { get; set; }
}
