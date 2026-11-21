using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Products;

public sealed class ProductsView : RecordPage<Product>
{
    protected override bool HasHistory => true;

    public ProductsView(AppServices services) : base(services, "Products", "products")
    {
        Ui.Toggle(Toolbar, "Show images", true, value =>
        {
            ShowImages = value;
            Render();
        });
        Toolbar.Add(Ui.Button("Import CSV", "upload", () => Import("products/importcsv", "Import CSV")));
        Toolbar.Add(Ui.Actions(Ui.Button("Export CSV", "download", () => services.Download("products/exportascsv", "Products.csv")), Ui.Button("Export PDF", "download", () => services.Download("products/exportaspdf", "Products.pdf"))));
    }

    protected override View Form(Product value, Action changed)
    {
        var form = Ui.Stack();
        Ui.Field(form, "Name", value.Name, text =>
        {
            value.Name = text;
            changed();
        });
        Ui.Field(form, "Code", value.Code, text =>
        {
            value.Code = text;
            changed();
        });
        Ui.Field(form, "Description", value.Description, text =>
        {
            value.Description = text;
            changed();
        });
        return form;
    }

    protected override string Subtitle(Product value) => value.Code + "\n" + value.Description;
    protected override View Details(Product value) => Ui.Stack(Ui.Text(value.Name, true), Ui.Text(value.Code), Ui.Text(value.Description), Ui.Text($"Price: {value.Price:0.00}"), Ui.Text($"Rating: {value.StarRating:0.#} / 5"), Ui.Text("Image URL: " + value.ImageUrl));
}
