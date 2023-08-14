using ClassifiedAds.Application;
using ClassifiedAds.Application.Users.Services;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.UnitTests.Application.Services;

public class UserServiceTests
{
    private Mock<IRepository<User, Guid>> _userRepository;
    private UserService _userService;

    public UserServiceTests()
    {
        _userRepository = new Mock<IRepository<User, Guid>>();
        var serviceProvider = new Mock<IServiceProvider>();
        var dispatcher = new Dispatcher(serviceProvider.Object);
        _userService = new UserService(_userRepository.Object, dispatcher);
    }

    [Fact]
    public async Task GetUserById_ThereIsNoUser_ReturnNull()
    {
        // Arrange
        var users = new List<User>();
        _userRepository.Setup(x => x.GetQueryableSet()).Returns(users.AsQueryable());

        // Act
        var user = await _userService.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserById_InvalidId_ThrowException()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act && Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await _userService.GetByIdAsync(userId));
    }

    [Fact]
    public async Task GetUserById_ThereIsUser_ReturnOne()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<User> { new User { Id = userId, UserName = "XXX" } }.AsQueryable();
        _userRepository.Setup(x => x.GetQueryableSet()).Returns(users);
        _userRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<IQueryable<User>>()))
            .Returns(Task.FromResult(users.FirstOrDefault(x => x.Id == userId)));

        // Act
        var user = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal("XXX", user.UserName);
    }
}
