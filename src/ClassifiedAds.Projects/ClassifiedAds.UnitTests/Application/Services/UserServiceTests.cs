using ClassifiedAds.Application.Services;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClassifiedAds.UnitTests.Application.Services
{
    public class UserServiceTests
    {
        private Mock<IRepository<User, Guid>> _userRepository;
        private UserService _userService;

        public UserServiceTests()
        {
            _userRepository = new Mock<IRepository<User, Guid>>();
            var domainEvents = new Mock<IDomainEvents>();
            _userService = new UserService(_userRepository.Object, domainEvents.Object);
        }

        [Fact]
        public void GetUserById_ThereIsNoUser_ReturnNull()
        {
            // Arrange
            var users = new List<User>();
            _userRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());

            // Act
            var user = _userService.GetById(Guid.NewGuid());

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void GetUserById_InvalidId_ThrowException()
        {
            // Arrange
            var userId = Guid.Empty;

            // Act && Assert
            Assert.Throws<ValidationException>(() => _userService.GetById(userId));
        }

        [Fact]
        public void GetUserById_ThereIsUser_ReturnOne()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User> { new User { Id = userId, UserName = "XXX" } };
            _userRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());

            // Act
            var user = _userService.GetById(userId);

            // Assert
            Assert.Equal(userId, user.Id);
            Assert.Equal("XXX", user.UserName);
        }
    }
}
