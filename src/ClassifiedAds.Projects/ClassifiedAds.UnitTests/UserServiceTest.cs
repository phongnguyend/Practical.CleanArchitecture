using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.UnitTest
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IRepository<User>> _userRepository;
        private UserService _userService;

        public UserServiceTest()
        {
            _userRepository = new Mock<IRepository<User>>();
            _userService = new UserService(_userRepository.Object);
        }

        [TestMethod]
        public void GetUserById_ThereIsNoUser_ReturnNull()
        {
            //Arrange
            var users = new List<User>();
            _userRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());

            //Act
            var user = _userService.GetUserById(Guid.NewGuid());

            //Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public void GetUserById_InvalidId_ThrowException()
        {
            //Arrange
            var userId = Guid.Empty;

            //Act && Assert
            Assert.ThrowsException<ValidationException>(() => _userService.GetUserById(userId));
        }

        [TestMethod]
        public void GetUserById_ThereIsUser_ReturnOne()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var users = new List<User> { new User { Id = userId.ToString(), UserName = "XXX" } };
            _userRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable());

            //Act
            var user = _userService.GetUserById(userId);

            //Assert
            Assert.AreEqual(userId.ToString(), user.Id);
            Assert.AreEqual("XXX", user.UserName);
        }
    }
}
