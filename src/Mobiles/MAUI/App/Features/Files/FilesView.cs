using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Core.Network;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Files;

public sealed class FilesView(AppServices services) : RecordPage<StoredFile>(services, "Files", "files")
{
    private FileResult? selected;
    protected override bool HasHistory => true;

    protected override View Form(StoredFile value, Action changed)
    {
        selected = null;
        var form = Ui.Stack();
        Ui.Field(form, "Name", value.Name, text =>
        {
            value.Name = text;
            changed();
        });
        Ui.Field(form, "Description", value.Description, text =>
        {
            value.Description = text;
            changed();
        });
        if (string.IsNullOrEmpty(value.Id))
        {
            Ui.Toggle(form, "Encrypted", value.Encrypted, v =>
            {
                value.Encrypted = v;
                changed();
            });
            var name = Ui.Text("No file selected");
            form.Add(Ui.Button("Choose file", "upload", async () =>
            {
                selected = await FilePicker.Default.PickAsync();
                name.Text = selected?.FileName ?? "No file selected";
                changed();
            }));
            form.Add(name);
        }
        else
        {
            form.Add(Ui.Text("Filename: " + value.FileName));
            form.Add(Ui.Text("Encrypted: " + value.Encrypted));
        }

        return form;
    }

    protected override string? Validate(StoredFile value) => value.Validate() ?? (string.IsNullOrEmpty(value.Id) && selected is null ? "Choose a file." : null);
    protected override async Task Save(StoredFile value)
    {
        if (!string.IsNullOrEmpty(value.Id))
        {
            await base.Save(value);
            return;
        }

        await using var stream = await selected!.OpenReadAsync();
        await Services.Api.Upload("files", stream, selected.FileName, new Dictionary<string, string> { ["name"] = value.Name, ["description"] = value.Description, ["encrypted"] = value.Encrypted.ToString().ToLowerInvariant() });
    }

    protected override string Subtitle(StoredFile value) => value.Description + "\n" + value.FileName;
    protected override View Details(StoredFile value) => Ui.Stack(Ui.Text(value.Name, true), Ui.Text(value.Description), Ui.Text("Filename: " + value.FileName), Ui.Text($"Size: {value.Size} bytes"), Ui.Text("Encrypted: " + value.Encrypted), Ui.Text("Uploaded: " + value.UploadedTime), Ui.Text("Location: " + value.FileLocation));
    protected override View ExtraActions(StoredFile value) => Ui.Button("Download", "download", () => Services.Download(ApiClient.PathFor("files", value.Id) + "/download", value.FileName));
}
