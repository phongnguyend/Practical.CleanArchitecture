using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Roles;

public sealed class RolesView(AppServices services) : RecordPage<Role>(services, "Roles", "roles")
{
    protected override View Form(Role value, Action changed)
    {
        var form = Ui.Stack();
        Ui.Field(form, "Name", value.Name, text =>
        {
            value.Name = text;
            changed();
        });
        return form;
    }

    protected override string Subtitle(Role value) => value.NormalizedName;
    protected override View Details(Role value) => Ui.Stack(Ui.Text(value.Name, true), Ui.Text("Normalized name: " + value.NormalizedName), Ui.Text("ID: " + value.Id));
}
