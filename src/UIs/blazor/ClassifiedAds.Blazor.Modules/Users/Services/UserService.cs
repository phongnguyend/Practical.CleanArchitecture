using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Users.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Users.Services;

public class UserService : HttpService
{
    public UserService(HttpClient httpClient, ITokenManager tokenManager)
        : base(httpClient, tokenManager)
    {
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await GetAsync<List<UserModel>>("api/users");
        return users;
    }

    public async Task<UserModel> GetUserByIdAsync(Guid id)
    {
        var user = await GetAsync<UserModel>($"api/users/{id}");
        return user;
    }

    public async Task<UserModel> CreateUserAsync(UserModel product)
    {
        var createdUser = await PostAsync<UserModel>("api/users", product);
        return createdUser;
    }

    public async Task<UserModel> UpdateUserAsync(Guid id, UserModel product)
    {
        var updatedUser = await PutAsync<UserModel>($"api/users/{id}", product);
        return updatedUser;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await DeleteAsync($"api/users/{id}");
    }

    public async Task SetPasswordAsync(SetPasswordModel model)
    {
        await PutAsync<string>($"api/users/{model.Id}/password", model);
    }

    public async Task SendPasswordResetEmailAsync(Guid id)
    {
        await PostAsync<string>($"api/users/{id}/passwordresetemail");
    }

    public async Task SendEmailAddressConfirmationEmailAsync(Guid id)
    {
        await PostAsync<string>($"api/users/{id}/emailaddressconfirmation");
    }
}
