using ClassifiedAds.Blazor.Modules.Users.Models;
using ClassifiedAds.Blazor.Modules.Users.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Users.Components
{
    public partial class AddEdit
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public UserService UserService { get; set; }


        [Parameter]
        public UserModel User { get; set; } = new UserModel();

        protected async Task HandleValidSubmit()
        {
            var user = await (User.Id == Guid.Empty ? UserService.CreateUser(User) : UserService.UpdateUser(User.Id, User));
            NavManager.NavigateTo($"/users/{user.Id}");
        }
    }
}
