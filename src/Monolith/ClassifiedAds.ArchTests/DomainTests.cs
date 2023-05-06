using ClassifiedAds.Domain.Entities;
using NetArchTest.Rules;
using Xunit;

namespace ClassifiedAds.ArchTests;

public class DomainTests
{
    [Fact]
    public void DomainShouldNotHaveDependencyOnApplication()
    {
        var result = Types.InAssembly(typeof(Entity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Application")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DomainShouldNotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(Entity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DomainShouldNotHaveDependencyOnPersistence()
    {
        var result = Types.InAssembly(typeof(Entity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOn("ClassifiedAds.Persistence")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
