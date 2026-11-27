using System.Text;
using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class ProductsTests : TestBase
{
    [UiTheory]
    [Apps]
    public Task RequiredFields(string app) => Run(app, async s =>
    {
        await s.Open("Products");
        await s.Click("Add Product");
        await s.Click("Save");
        await Checks.Invalid(s);
        await s.Path("/products/add");
    });

    [UiTheory]
    [Apps]
    public Task CsvValidationAndCancel(string app) => Run(app, s => Checks.ImportValidation(s, "Products", "Import Csv"));

    [UiTheory]
    [Apps]
    public Task CsvDownload(string app) => Run(app, async s =>
    {
        await s.Open("Products");
        Checks.Download(await s.Download("Export as Csv"), ".csv");
    });

    [UiTheory]
    [Apps]
    public Task CsvImportResets(string app) => Run(app, async s =>
    {
        await s.Open("Products");
        var file = s.Fixture("products.csv", Encoding.UTF8.GetBytes("Code,Name,Description\nPWCSV,Import preview,Parsed by .NET UI tests\n"));
        await Checks.ImportAndReset(s, "Import Csv", file);
    });

    [UiTheory("E2E_PDF")]
    [Apps]
    public Task PdfDownload(string app) => Run(app, async s =>
    {
        await s.Open("Products");
        var file = await s.Download("Export as Pdf");
        Checks.Download(file, ".pdf");
        Assert.StartsWith("%PDF", Encoding.ASCII.GetString(File.ReadAllBytes(file)[..4]));
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task CrudFilterImagesDetailsAndAudit(string app) => Run(app, async s =>
    {
        s.Own("products");
        var name = s.Prefix + "-product";
        await s.Open("Products");
        await s.Click("Add Product");
        await s.Fill("Name", name);
        await s.Fill("Code", "PW123");
        await s.Fill("Description", "Created by .NET UI tests");
        await s.Response("products", "POST", () => s.Click("Save"));
        var row = await s.Find("products", name);
        var id = row["id"]!.GetValue<string>();
        await s.Path("/products/" + id);
        await s.Open("Products");
        await s.Fill("Filter records", name);
        await Checks.Count(s, Selectors.Row(name), 1);
        await s.Click("Show Image");
        await s.Browser.Visible(Selectors.Row(name) + "//img");
        await s.Click("Hide Image");
        await Checks.Count(s, Selectors.Row(name) + "//img", 0);
        await s.Click(name, Selectors.Row(name));
        await s.Path("/products/" + id);
        await s.Browser.Visible(Selectors.Text("Created by .NET UI tests"));
        await s.Click("Back");
        await s.Click("Edit", Selectors.Row(name));
        await s.Browser.Value(Selectors.Field("Name"), name);
        await s.Fill("Description", "Updated by .NET UI tests");
        await s.Response("products/" + id, "PUT", () => s.Click("Save"));
        await s.Path("/products/" + id);
        await s.Open("Products");
        await s.Browser.Contains(Selectors.Row(name), "Updated by .NET UI tests");
        await Checks.History(s, name, "products", id);
        await s.Click("Delete", Selectors.Row(name));
        await s.Browser.Contains(Selectors.Dialog, name);
        await s.Click("No", Selectors.Dialog);
        await s.Browser.Visible(Selectors.Row(name));
        await s.Fill("Filter records", s.Prefix + "-missing");
        await Checks.Count(s, "//table/tbody/tr", 0);
        await s.Fill("Filter records", name);
        await s.Delete(name, "products", id);
    });
}
