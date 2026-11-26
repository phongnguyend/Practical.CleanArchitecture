using ClassifiedAds.Blazor.Modules.Core.Components;
using ClassifiedAds.Blazor.Modules.Users.Components;
using ClassifiedAds.Blazor.Modules.Users.Models;
using ClassifiedAds.Blazor.Modules.Users.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Users.Pages;

public partial class Detail
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public UserService UserService { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    public UserModel User { get; set; } = new UserModel();

    public SetPasswordDialog SetPasswordDialog { get; set; }

    public ConfirmDialog SendPasswordResetEmailDialog { get; set; }

    public ConfirmDialog SendEmailAddressConfirmationEmailDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        User = await UserService.GetUserByIdAsync(Id);
    }

    protected async void ConfirmedSendPasswordResetEmail(bool confirmed)
    {
        await UserService.SendPasswordResetEmailAsync(User.Id);
        SendPasswordResetEmailDialog.Close();
    }

    protected async void ConfirmedSendEmailAddressConfirmation(bool confirmed)
    {
        await UserService.SendEmailAddressConfirmationEmailAsync(User.Id);
        SendEmailAddressConfirmationEmailDialog.Close();
    }
}
