using System.Text;
using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class FilesTests : TestBase
{
    [UiTheory]
    [Apps("1440", "390")]
    public Task FilePickerSelection(string app, string width) => Run(app, async s =>
    {
        await s.Browser.Resize(int.Parse(width), 900);
        await s.Open("Files");
        await s.Click("Upload File");
        var file = s.Fixture("sample.txt", Encoding.UTF8.GetBytes("File picker preview"));
        await s.Browser.Wait(async () => (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".length"))!.GetValue<int>() > 0, "File input missing");
        var count = (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".length"))!.GetValue<int>();
        for (var index = 1; index <= count; index++)
        {
            var input = "(" + Selectors.Files + ")[" + index + "]";
            await s.Browser.Upload(input, file);
            await s.Browser.Wait(async () => (await s.Browser.Evaluate($"{UiBrowser.Element(input)}.files[0]?.name"))?.GetValue<string>() == "sample.txt", "Selected filename missing");
        }

        foreach (var theme in new[] { "Light", "Dark" })
        {
            await s.Theme(theme);
            await Checks.NoOverflow(s);
            await s.Browser.Screenshot(Path.Combine(s.Directory, "upload-" + theme + "-" + width + ".png"));
        }
    });

    [UiTheory]
    [Apps]
    public Task RequiredFields(string app) => Run(app, async s =>
    {
        await s.Open("Files");
        await s.Click("Upload File");
        await s.Click("Save");
        await s.Path("/files/upload");
        await Checks.Invalid(s);
    });

    [UiTheory]
    [Apps]
    public Task MissingFileHighlightAndRecovery(string app) => Run(app, async s =>
    {
        await s.Open("Files");
        await s.Click("Upload File");
        await s.Browser.Wait(async () => (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".length"))!.GetValue<int>() > 0, "File input missing");
        var count = (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".length"))!.GetValue<int>();
        for (var index = 1; index <= count; index++)
        {
            await s.Open("Files");
            await s.Click("Upload File");
            await s.Fill("Name", "File validation check");
            await s.Fill("Description", "No upload should be sent");
            await s.Click("Save");
            await s.Path("/files/upload");
            await s.Browser.Wait(async () => (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".every(e => e.matches(':invalid,.is-invalid,[aria-invalid=true]'))"))!.GetValue<bool>(), "Missing file highlight absent");
            await s.Browser.Upload("(" + Selectors.Files + ")[" + index + "]", s.Fixture("validation.txt", Encoding.UTF8.GetBytes("Validation fixture")));
            await s.Browser.Wait(async () => (await s.Browser.Evaluate(UiBrowser.Elements(Selectors.Files) + ".every(e => !e.matches(':invalid,.is-invalid,[aria-invalid=true]'))"))!.GetValue<bool>(), "File error did not clear");
        }
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps("false", "true")]
    public Task UploadEditDownloadHistoryAndDelete(string app, string encrypted) => Run(app, async s =>
    {
        s.Own("files");
        var name = s.Prefix + "-file";
        var content = Encoding.UTF8.GetBytes("Fixture " + name + "\n").Concat(new byte[] { 0, 128, 255, 13, 10 }).ToArray();
        await s.Open("Files");
        await s.Click("Upload File");
        await s.Fill("Name", name);
        await s.Fill("Description", "E2E upload");
        await s.Browser.Upload("(" + Selectors.Files + ")[1]", s.Fixture(name + ".bin", content));
        await s.Browser.Check(Selectors.Field("Encrypted"), bool.Parse(encrypted));
        await s.Response("files", "POST", () => s.Click("Save"));
        var row = await s.Find("files", name);
        var id = row["id"]!.GetValue<string>();
        await s.Path("/files/edit/" + id);
        await s.Browser.Value(Selectors.Field("Name"), name);
        await s.Browser.Value(Selectors.Field("Description"), "E2E upload");
        await s.Fill("Description", "E2E updated upload");
        await s.Response("files/" + id, "PUT", () => s.Click("Save"));
        await s.Path("/files");
        await s.Browser.Contains(Selectors.Row(name), "E2E updated upload");
        var download = await s.Browser.Download(Selectors.Action("Download", Selectors.Row(name)), Path.Combine(s.Directory, "download"));
        Assert.Equal(name + ".bin", Path.GetFileName(download));
        Assert.Equal(content, File.ReadAllBytes(download));
        await Checks.History(s, name, "files", id);
        await s.Delete(name, "files", id);
    });
}
