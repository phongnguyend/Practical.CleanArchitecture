using ClassifiedAds.Blazor.Modules.Settings.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Settings.Components;

public partial class AddEditDialog
{
    protected string Title { get; set; }

    protected bool ShowDialog { get; set; }

    [Parameter]
    public EventCallback<ConfigurationEntryModel> ConfirmEventCallback { get; set; }

    public ConfigurationEntryModel ConfigurationEntry { get; private set; }

    public void Show(ConfigurationEntryModel model)
    {
        ConfigurationEntry = model;
        Title = ConfigurationEntry.Id == Guid.Empty ? "Add" : "Edit";
        ShowDialog = true;
        StateHasChanged();
    }

    public void Close()
    {
        ShowDialog = false;
        StateHasChanged();
    }

    protected async Task HandleValidSubmit()
    {
        await ConfirmEventCallback.InvokeAsync(ConfigurationEntry);
    }
}
