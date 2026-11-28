using ClassifiedAds.ApiIntegrationTests.Models.Roles;
using ClassifiedAds.ApiIntegrationTests.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.ApiIntegrationTests.WebAPI;

public class RoleMembershipTests : TestBase
{
    private static readonly string[] MemberFields = { "email", "id", "userName" };

    [Fact]
    public async Task AssignmentRemovalAndUserRolesAreConsistent()
    {
        await GetTokenAsync();
        var prefix = $"e2e-membership-{Guid.NewGuid():N}";
        UserModel user = null;
        RoleModel role = null;
        RoleModel other = null;
        try
        {
            user = await PostAsync<UserModel>("api/users", new UserModel
            {
                UserName = prefix,
                Email = $"{prefix}@example.invalid",
            });
            role = await PostAsync<RoleModel>("api/roles", new RoleModel { Name = $"{prefix}-role" });
            other = await PostAsync<RoleModel>("api/roles", new RoleModel { Name = $"{prefix}-other" });

            Assert.Empty(await GetAsync<List<UserModel>>($"api/roles/{role.Id}/users"));
            Assert.Empty(await GetAsync<List<RoleModel>>($"api/users/{user.Id}/roles"));
            for (var attempt = 0; attempt < 2; attempt++)
            {
                await AssertStatus(HttpMethod.Put, $"api/roles/{role.Id}/users/{user.Id}", HttpStatusCode.NoContent);
            }

            using (var response = await _httpClient.GetAsync($"api/roles/{role.Id}/users"))
            {
                response.EnsureSuccessStatusCode();
                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var member = Assert.Single(json.RootElement.EnumerateArray());
                Assert.Equal(user.Id, member.GetProperty("id").GetGuid());
                Assert.Equal(user.UserName, member.GetProperty("userName").GetString());
                Assert.Equal(user.Email, member.GetProperty("email").GetString());
                Assert.Equal(MemberFields, member.EnumerateObject().Select(property => property.Name).OrderBy(name => name));
            }

            await AssertStatus(HttpMethod.Put, $"api/roles/{other.Id}/users/{user.Id}", HttpStatusCode.NoContent);
            var assigned = await GetAsync<List<RoleModel>>($"api/users/{user.Id}/roles");
            Assert.Equal(2, assigned.Count);
            Assert.Contains(assigned, item => item.Id == role.Id && item.Name == role.Name);
            Assert.Contains(assigned, item => item.Id == other.Id && item.Name == other.Name);

            for (var attempt = 0; attempt < 2; attempt++)
            {
                await AssertStatus(HttpMethod.Delete, $"api/roles/{role.Id}/users/{user.Id}", HttpStatusCode.NoContent);
            }

            Assert.Empty(await GetAsync<List<UserModel>>($"api/roles/{role.Id}/users"));
            Assert.Equal(other.Id, Assert.Single(await GetAsync<List<RoleModel>>($"api/users/{user.Id}/roles")).Id);

            await AssertStatus(HttpMethod.Put, $"api/roles/{role.Id}/users/{Guid.NewGuid()}", HttpStatusCode.NotFound);
            await AssertStatus(HttpMethod.Put, $"api/roles/{Guid.NewGuid()}/users/{user.Id}", HttpStatusCode.NotFound);
            await AssertStatus(HttpMethod.Get, $"api/users/{Guid.NewGuid()}/roles", HttpStatusCode.NotFound);
            await AssertStatus(HttpMethod.Get, $"api/roles/{Guid.NewGuid()}/users", HttpStatusCode.NotFound);
        }
        finally
        {
            // Delete only IDs returned for this test's uniquely named fixtures.
            try
            {
                if (other != null)
                {
                    await DeleteAsync($"api/roles/{other.Id}");
                }
            }
            finally
            {
                try
                {
                    if (role != null)
                    {
                        await DeleteAsync($"api/roles/{role.Id}");
                    }
                }
                finally
                {
                    if (user != null)
                    {
                        await DeleteAsync($"api/users/{user.Id}");
                    }
                }
            }
        }
    }

    private async Task AssertStatus(HttpMethod method, string path, HttpStatusCode expected)
    {
        using var request = new HttpRequestMessage(method, path);
        using var response = await _httpClient.SendAsync(request);
        Assert.Equal(expected, response.StatusCode);
    }
}
