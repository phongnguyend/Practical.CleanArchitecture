using ClassifiedAds.Application;
using NetArchTest.Rules;
using Xunit;

namespace ClassifiedAds.ArchTests;

public class ApplicationTests
{
    [Fact]
    public void ApplicationShouldNotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(Dispatcher).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void ApplicationShouldNotHaveDependencyOnPersistence()
    {
        var result = Types.InAssembly(typeof(Dispatcher).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Persistence")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
