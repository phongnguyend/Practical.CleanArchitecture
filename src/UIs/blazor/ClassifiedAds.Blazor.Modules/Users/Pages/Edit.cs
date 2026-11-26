using ClassifiedAds.Blazor.Modules.Users.Models;
using ClassifiedAds.Blazor.Modules.Users.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Users.Pages;

public partial class Edit
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public UserService UserService { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    public UserModel User { get; set; } = new UserModel();

    protected override async Task OnInitializedAsync()
    {
        User = await UserService.GetUserByIdAsync(Id);
    }
}
