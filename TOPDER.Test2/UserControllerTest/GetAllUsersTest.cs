using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class GetAllUsersTest
    {
        // Khai báo các trường cho mock objects
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<ISendMailService> _sendMailServiceMock;
        private Mock<JwtHelper> _jwtHelperMock;
        private Mock<IAdminService> _adminServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IIdentityService> _identityServiceMock;
        private Mock<IUserOtpRepository> _userOtpRepositoryMock;

        private UserController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Khởi tạo các Mock cho dịch vụ và repository
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _userServiceMock = new Mock<IUserService>();
            _customerServiceMock = new Mock<ICustomerService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _walletServiceMock = new Mock<IWalletService>();
            _sendMailServiceMock = new Mock<ISendMailService>();
            _jwtHelperMock = new Mock<JwtHelper>();
            _adminServiceMock = new Mock<IAdminService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _userOtpRepositoryMock = new Mock<IUserOtpRepository>();

            // Khởi tạo UserController với các mock objects
            _controller = new UserController(
                _restaurantServiceMock.Object,
                _cloudinaryServiceMock.Object,
                _sendMailServiceMock.Object,
                _userServiceMock.Object,
                _customerServiceMock.Object,
                _walletServiceMock.Object,
                _jwtHelperMock.Object,
                _adminServiceMock.Object,
                _userRepositoryMock.Object,
                _identityServiceMock.Object,
                _userOtpRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task GetAllUsers_Success_ReturnsOk()
        {
            // Arrange
            var users = new List<UserLoginDTO>
            {
                new UserLoginDTO { Uid = 1, Email = "test1@example.com" },
                new UserLoginDTO { Uid = 2, Email = "test2@example.com" }
            };

            // Mock the GetAllUsersAsync method to return a list of users
            _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;  // Expect OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure that the result is OkObjectResult
            var returnedUsers = okResult.Value as List<UserLoginDTO>;  // Get the list of users from the result
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedUsers);  // Ensure the returned value is a List of Users
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedUsers.Count);  // Ensure the count of returned users is correct
        }

        [TestMethod]
        public async Task GetAllUsers_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var exceptionMessage = "Error retrieving users";
            _userServiceMock.Setup(service => service.GetAllUsersAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var statusCodeResult = result as ObjectResult;  // Expect ObjectResult to be returned (which wraps the status code and message)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult); // Ensure that the result is an ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult.StatusCode); // Ensure the status code is 500
            var errorMessage = statusCodeResult.Value as string;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(errorMessage.Contains("Error retrieving users")); // Ensure the error message contains the expected text
        }
    }
}
