using ClassifiedAds.WebAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.IntegrationTests.WebAPI;

public class UsersControllerTests : TestBase
{
    public UsersControllerTests()
        : base()
    {
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    private async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await GetAsync<List<UserModel>>("api/users");
        return users;
    }

    private async Task<UserModel> GetUserByIdAsync(Guid id)
    {
        var user = await GetAsync<UserModel>($"api/users/{id}");
        return user;
    }

    private async Task<UserModel> CreateUserAsync(UserModel user)
    {
        var createdUser = await PostAsync<UserModel>("api/users", user);
        return createdUser;
    }

    private async Task<UserModel> UpdateUserAsync(Guid id, UserModel user)
    {
        var updatedUser = await PutAsync<UserModel>($"api/users/{id}", user);
        return updatedUser;
    }

    private async Task DeleteUserAsync(Guid id)
    {
        await DeleteAsync($"api/users/{id}");
    }

    private async Task SetPasswordAsync(SetPasswordModel model)
    {
        await PutAsync<string>($"api/users/{model.Id}/password", model);
    }

    private async Task SendPasswordResetEmailAsync(Guid id)
    {
        await PostAsync<string>($"api/users/{id}/passwordresetemail");
    }

    private async Task SendEmailAddressConfirmationEmailAsync(Guid id)
    {
        await PostAsync<string>($"api/users/{id}/emailaddressconfirmation");
    }

    [Fact]
    public async Task AllInOne()
    {
        await GetTokenAsync();

        // POST
        var user = new UserModel
        {
            UserName = "phong1@gmail.com",
            Email = "phong1@gmail.com",
        };
        UserModel createdUser = await CreateUserAsync(user);
        Assert.True(user.Id != createdUser.Id);
        Assert.Equal(user.UserName, createdUser.UserName);
        Assert.Equal(user.Email, createdUser.Email);

        // GET
        var users = await GetUsersAsync();
        Assert.True(users.Count > 0);

        // GET ONE
        var refreshedUser = await GetUserByIdAsync(createdUser.Id);
        Assert.Equal(refreshedUser.Id, createdUser.Id);
        Assert.Equal(refreshedUser.UserName, createdUser.UserName);
        Assert.Equal(refreshedUser.Email, createdUser.Email);

        // PUT
        refreshedUser.Email = "phong2@gmail.com";
        var updatedUser = await UpdateUserAsync(refreshedUser.Id, refreshedUser);
        Assert.Equal(refreshedUser.Id, updatedUser.Id);
        Assert.Equal("phong2@gmail.com", updatedUser.Email);

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await SetPasswordAsync(new SetPasswordModel
            {
                Id = createdUser.Id,
                Password = "abc",
                ConfirmPassword = "abc",
            });
        });

        await SetPasswordAsync(new SetPasswordModel
        {
            Id = createdUser.Id,
            Password = "v*7Un8b4rcN@<-RN",
            ConfirmPassword = "v*7Un8b4rcN@<-RN",
        });

        await SendEmailAddressConfirmationEmailAsync(createdUser.Id);
        await SendPasswordResetEmailAsync(createdUser.Id);

        // DELETE
        await DeleteUserAsync(createdUser.Id);
        Assert.Null(await GetUserByIdAsync(createdUser.Id));
    }
}
