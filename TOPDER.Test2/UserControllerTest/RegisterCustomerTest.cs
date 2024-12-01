using Microsoft.AspNetCore.Http;
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
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class RegisterCustomerTest
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
        private Mock<IRoleRepository> _roleRepositoryMock;

        private UserController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Khởi tạo các mock objects
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
            _roleRepositoryMock = new Mock<IRoleRepository>();

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
                _userOtpRepositoryMock.Object,
                _roleRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task RegisterCustomer_Success_ReturnsOkResult()
        {
            // Arrange
            var customerRequest = new CreateCustomerRequest
            {
                Email = "customer@example.com",
                Password = "Password123",
                Name = "John Doe",
                Phone = "1234567890",
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male",
            };

            // Mock the user service to return a successful user creation
            var userDto = new User
            {
                Uid = 1,
                Email = customerRequest.Email,
                RoleId = 3, // Customer role
                Password = BCrypt.Net.BCrypt.HashPassword(customerRequest.Password),
                IsVerify = false,
                Status = Common_Status.ACTIVE,
                IsExternalLogin = false,
                CreatedAt = DateTime.Now
            };

            _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserDto>())).ReturnsAsync(userDto);

            // Mock the wallet service to return true indicating the wallet was created successfully
            _walletServiceMock.Setup(service => service.AddWalletBalanceAsync(It.IsAny<WalletBalanceDto>())).ReturnsAsync(true);

            // Mock the customer service to return a customer creation result
            var addedCustomer = new Customer { Uid = 1, Name = customerRequest.Name };
            _customerServiceMock.Setup(service => service.AddAsync(It.IsAny<CreateCustomerRequest>())).ReturnsAsync(addedCustomer);

            // Mock the send mail service to simulate sending the email
            _sendMailServiceMock.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterCustomer(customerRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(addedCustomer, okResult.Value);
        }

        [TestMethod]
        public async Task RegisterCustomer_FailToCreateUser_ReturnsBadRequest()
        {
            // Arrange
            var customerRequest = new CreateCustomerRequest
            {
                Email = "customer@example.com",
                Password = "Password123",
                Name = "John Doe"
            };

            // Mock the user service to return null (simulating a failure to create the user)
            _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserDto>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.RegisterCustomer(customerRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo người dùng thất bại.", badRequestResult.Value);
        }
        [TestMethod]
        public async Task RegisterCustomer_FailToCreateWallet_ReturnsBadRequest()
        {
            // Arrange
            var customerRequest = new CreateCustomerRequest
            {
                Email = "customer@example.com",
                Password = "Password123",
                Name = "John Doe"
            };

            // Mock the user service to return a valid user
            var userDto = new User
            {
                Uid = 1,
                Email = customerRequest.Email,
                RoleId = 3, // Customer role
                Password = BCrypt.Net.BCrypt.HashPassword(customerRequest.Password),
                IsVerify = false,
                Status = Common_Status.ACTIVE,
                IsExternalLogin = false,
                CreatedAt = DateTime.Now
            };

            _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserDto>())).ReturnsAsync(userDto);

            // Mock the wallet service to return false (indicating wallet creation failure)
            _walletServiceMock.Setup(service => service.AddWalletBalanceAsync(It.IsAny<WalletBalanceDto>())).ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterCustomer(customerRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tạo được Ví cho người dùng.", badRequestResult.Value);
        }
        [TestMethod]
        public async Task RegisterCustomer_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var customerRequest = new CreateCustomerRequest
            {
                Email = "customer@example.com",
                Password = "Password123",
                Name = "John Doe"
            };

            // Mock the user service to throw an exception
            _userServiceMock.Setup(service => service.AddAsync(It.IsAny<UserDto>())).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.RegisterCustomer(customerRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));

            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create restaurant: Unexpected error", objectResult.Value);
        }

    }
}
