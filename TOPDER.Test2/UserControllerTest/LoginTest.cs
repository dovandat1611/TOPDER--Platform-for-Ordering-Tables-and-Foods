using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

using TOPDER.Repository.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TOPDER.Service.Services;

using AutoMapper;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class LoginTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public async Task GetUserByEmailAndPassword_ValidUser_ReturnsUserLoginDTO()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };

            var user = new User
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsVerify = true,
                Status = Common_Status.ACTIVE
            };



            // Act
            var result = await _userService.GetUserByEmailAndPassword(loginModel);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(user.Email, result.Email);
        }



        [TestMethod]
        public async Task GetUserByEmailAndPassword_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "wrongpassword" };

            // Create a valid user with ACTIVE status and verified email
            var user = new User
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),  // The actual password
                IsVerify = true,
                Status = Common_Status.ACTIVE
            };

            // Mock the repository to return the user
            _userRepositoryMock.Setup(r => r.QueryableAsync())
                .Returns(Task.FromResult(new List<User> { user }.AsQueryable())); // Corrected setup for IQueryable

            // Act
            await _userService.GetUserByEmailAndPassword(loginModel);  // Should throw UnauthorizedAccessException
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task GetUserByEmailAndPassword_UnverifiedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };

            // Create a valid user with ACTIVE status but unverified email
            var user = new User
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsVerify = false,  // Unverified
                Status = Common_Status.ACTIVE
            };

            // Mock the repository to return the user
            _userRepositoryMock.Setup(r => r.QueryableAsync())
                .Returns(Task.FromResult(new List<User> { user }.AsQueryable())); // Corrected setup for IQueryable

            // Act
            await _userService.GetUserByEmailAndPassword(loginModel);  // Should throw UnauthorizedAccessException
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task GetUserByEmailAndPassword_InactiveUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password123" };

            // Create a valid user with INACTIVE status
            var user = new User
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsVerify = true,
                Status = Common_Status.INACTIVE  // Inactive status
            };

            // Mock the repository to return the user
            _userRepositoryMock.Setup(r => r.QueryableAsync())
                .Returns(Task.FromResult(new List<User> { user }.AsQueryable())); // Corrected setup for IQueryable

            // Act
            await _userService.GetUserByEmailAndPassword(loginModel);  // Should throw UnauthorizedAccessException
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task GetUserByEmailAndPassword_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "nonexistent@example.com", Password = "password123" };

            // Mock the repository to return an empty list
            _userRepositoryMock.Setup(r => r.QueryableAsync())
                .Returns(Task.FromResult(new List<User>().AsQueryable())); // Corrected setup for IQueryable

            // Act
            await _userService.GetUserByEmailAndPassword(loginModel);  // Should throw UnauthorizedAccessException
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task GetUserByEmailAndPassword_UserFoundButIncorrectPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "wrongpassword" };

            // Create a valid user with ACTIVE status and verified email
            var user = new User
            {
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),  // The actual password
                IsVerify = true,
                Status = Common_Status.ACTIVE
            };

            // Mock the repository to return the user
            _userRepositoryMock.Setup(r => r.QueryableAsync())
                .Returns(Task.FromResult(new List<User> { user }.AsQueryable())); // Corrected setup for IQueryable

            // Act
            await _userService.GetUserByEmailAndPassword(loginModel);  // Should throw UnauthorizedAccessException
        }
    }
}
