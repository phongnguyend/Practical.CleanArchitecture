using ClassifiedAds.Blazor.Modules.Core.Components;
using ClassifiedAds.Blazor.Modules.Settings.Components;
using ClassifiedAds.Blazor.Modules.Settings.Models;
using ClassifiedAds.Blazor.Modules.Settings.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Settings.Pages;

public partial class List
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public ILogger<List> Logger { get; set; }

    [Inject]
    public ConfigurationEntryService ConfigurationEntryService { get; set; }

    protected AddEditDialog AddEditDialog { get; set; }

    protected ConfirmDialog DeleteDialog { get; set; }

    public List<ConfigurationEntryModel> ConfigurationEntries { get; set; } = new List<ConfigurationEntryModel>();

    public ConfigurationEntryModel DeletingConfigurationEntry { get; private set; }

    protected void AddSetting()
    {
        AddEditDialog.Show(new ConfigurationEntryModel());
    }

    protected void EditSetting(ConfigurationEntryModel model)
    {
        AddEditDialog.Show(new ConfigurationEntryModel
        {
            Id = model.Id,
            Key = model.Key,
            Value = model.IsSensitive ? string.Empty : model.Value,
            Description = model.Description,
            IsSensitive = model.IsSensitive,
        });
    }

    protected void DeleteSetting(ConfigurationEntryModel model)
    {
        DeletingConfigurationEntry = model;
        DeleteDialog.Show();
    }

    protected override async Task OnInitializedAsync()
    {
        ConfigurationEntries = await ConfigurationEntryService.GetListAsync();
    }

    public async void ConfirmedDeleteSetting()
    {
        Logger.LogWarning($"Deleting Entry: {DeletingConfigurationEntry.Id}");

        await ConfigurationEntryService.DeleteAsync(DeletingConfigurationEntry.Id);
        DeleteDialog.Close();

        Logger.LogWarning($"Deleted Entry: {DeletingConfigurationEntry.Id}");

        ConfigurationEntries = await ConfigurationEntryService.GetListAsync();
        StateHasChanged();
    }

    public async void ConfirmedAddEdit(ConfigurationEntryModel model)
    {
        await (model.Id == Guid.Empty
            ? ConfigurationEntryService.CreateAsync(model)
            : ConfigurationEntryService.UpdateAsync(model));
        AddEditDialog.Close();
        ConfigurationEntries = await ConfigurationEntryService.GetListAsync();
        StateHasChanged();
    }

    protected async Task ExportAsExcel()
    {
        var token = await ConfigurationEntryService.GetAccessToken();
        await JSRuntime.Log(token);
        await JSRuntime.InvokeVoidAsync("interop.downloadFile", ConfigurationEntryService.GetExportExcelUrl(), token, "Settings.xlsx");
    }
}
