using ClassifiedAds.Blazor.Modules.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Roles.Services;

public class RoleModel
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string NormalizedName { get; set; }
}

public class RoleService : HttpService
{
    public RoleService(HttpClient client, ITokenManager tokenManager)
        : base(client, tokenManager)
    {
    }

    public Task<List<RoleModel>> LoadAsync() => GetAsync<List<RoleModel>>("api/roles");

    public Task<RoleModel> GetAsync(Guid id) => GetAsync<RoleModel>($"api/roles/{id}");

    public Task<RoleModel> SaveAsync(RoleModel role) => role.Id == Guid.Empty
        ? PostAsync<RoleModel>("api/roles", role)
        : PutAsync<RoleModel>($"api/roles/{role.Id}", role);

    public Task<List<ClassifiedAds.Blazor.Modules.Users.Models.UserModel>> GetMembersAsync(Guid id) => GetAsync<List<ClassifiedAds.Blazor.Modules.Users.Models.UserModel>>($"api/roles/{id}/users");

    public Task<List<RoleModel>> GetAssignedRolesAsync(Guid id) => GetAsync<List<RoleModel>>($"api/users/{id}/roles");

    public async Task SetMembershipAsync(Guid roleId, Guid userId, bool assigned)
    {
        await SetBearerToken();
        using var request = new HttpRequestMessage(assigned ? HttpMethod.Put : HttpMethod.Delete, $"api/roles/{roleId}/users/{userId}");
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public Task RemoveAsync(Guid id) => DeleteAsync($"api/roles/{id}");
}
