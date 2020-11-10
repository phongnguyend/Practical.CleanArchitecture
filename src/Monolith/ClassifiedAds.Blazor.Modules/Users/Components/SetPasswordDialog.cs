using ClassifiedAds.Blazor.Modules.Users.Models;
using ClassifiedAds.Blazor.Modules.Users.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Users.Components
{
    public partial class SetPasswordDialog
    {
        public bool ShowDialog { get; private set; }

        public SetPasswordModel SetPasswordModel { get; private set; }

        [Inject]
        public UserService UserService { get; set; }

        public void Show(UserModel userModel)
        {
            SetPasswordModel = new SetPasswordModel
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
            };
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
            await UserService.SetPassword(SetPasswordModel);
            Close();
        }
    }
}
