using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Identity.Grpc;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.Services.Identity.IntegrationTests.Grpc;

public class UserServiceTests
{
    private readonly User.UserClient _client;

    public UserServiceTests()
    {
        _client = new User.UserClient(ChannelFactory.Create("https://localhost:5001"));
    }

    [Fact]
    public async Task GetUsers()
    {
        var reply = await _client.GetUsersAsync(new GetUsersRequest());
    }
}
