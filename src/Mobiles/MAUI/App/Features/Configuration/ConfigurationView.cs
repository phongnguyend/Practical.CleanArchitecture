using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Configuration;

public sealed class ConfigurationView : RecordPage<ConfigurationEntry>
{
    public ConfigurationView(AppServices services) : base(services, "Configuration Entries", "configurationentries") => Toolbar.Add(Ui.Actions(Ui.Button("Import Excel", "upload", () => Import("configurationentries/ImportExcel", "Import Excel")), Ui.Button("Export Excel", "download", () => services.Download("configurationentries/ExportAsExcel", "ConfigurationEntries.xlsx"))));
    protected override View Form(ConfigurationEntry value, Action changed)
    {
        var form = Ui.Stack();
        Ui.Field(form, "Key", value.Key, text =>
        {
            value.Key = text;
            changed();
        });
        var entry = Ui.Field(form, "Value", value.Value, text =>
        {
            value.Value = text;
            changed();
        }, value.IsSensitive);
        Ui.Field(form, "Description", value.Description, text =>
        {
            value.Description = text;
            changed();
        });
        Ui.Toggle(form, "Sensitive", value.IsSensitive, v =>
        {
            value.IsSensitive = v;
            entry.IsPassword = v;
            changed();
        });
        return form;
    }

    protected override string Subtitle(ConfigurationEntry value) => (value.IsSensitive ? "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022" : value.Value) + "\n" + value.Description;
    protected override View Details(ConfigurationEntry value) => Ui.Stack(Ui.Text(value.Key, true), Ui.Text("Value: " + (value.IsSensitive ? "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022" : value.Value)), Ui.Text(value.Description), Ui.Text("Sensitive: " + value.IsSensitive), Ui.Text("Updated: " + value.UpdatedDateTime));
}
