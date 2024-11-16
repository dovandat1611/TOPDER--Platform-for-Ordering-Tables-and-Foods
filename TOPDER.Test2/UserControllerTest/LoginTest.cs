using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class LoginTest
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
        public async Task Login_ShouldReturnOkResult_WhenValidCredentialsForRestaurant()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "restaurant@example.com", Password = "validPassword" };
            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = "restaurant@example.com",
                Role = User_Role.RESTAURANT,
                CategoryRestaurantId = 1,
                NameOwner = "Restaurant Owner",
                NameRes = "Restaurant",
                Phone = "123456789",
                Logo = "logo.png",
                OpenTime = new TimeSpan(8, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Address = "123 Street",
                Description = "A great restaurant",
                Subdescription = "Best in town",
                ProvinceCity = "City",
                District = "District",
                Commune = "Commune",
                Discount = 10,
                MaxCapacity = 50,
                Price = 20.0m,
                IsBookingEnabled = true,
                FirstFeePercent = 5.0m,
                ReturningFeePercent = 5.0m,
                CancellationFeePercent = 10.0m
            };

            var restaurantInfo = new RestaurantInfoResponse
            {
                Uid = 1,
                Email = "restaurant@example.com",
                CategoryRestaurantId = 1,
                NameOwner = "Restaurant Owner",
                NameRes = "Restaurant",
                Phone = "123456789",
                Logo = "logo.png",
                OpenTime = new TimeSpan(8, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Address = "123 Street",
                Description = "A great restaurant",
                Subdescription = "Best in town",
                ProvinceCity = "City",
                District = "District",
                Commune = "Commune",
                Discount = 10,
                MaxCapacity = 50,
                Price = 20.0m,
                IsBookingEnabled = true,
                FirstFeePercent = 5.0m,
                ReturningFeePercent = 5.0m,
                CancellationFeePercent = 10.0m,
                Role = User_Role.RESTAURANT
            };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ReturnsAsync(userLoginDto);
            _jwtHelperMock.Setup(x => x.GenerateJwtToken(userLoginDto)).Returns("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as Repository.Entities.ApiResponse;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Success);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Authentication success.", response.Message);

            var data = response.Data as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.Token);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.UserInfo);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant", data.UserInfo.NameRes);
        }

        [TestMethod]
        public async Task Login_ShouldReturnOkResult_WhenValidCredentialsForAdmin()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "admin@example.com", Password = "validPassword" };
            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = "admin@example.com",
                Role = User_Role.ADMIN,
                Name = "Admin Name",
                Phone = "987654321",
                Image = "admin-image.jpg",
                Dob = new DateTime(1985, 5, 15)
            };

            var adminInfo = new AdminInfoRespone
            {
                Uid = 1,
                Email = "admin@example.com",
                Name = "Admin Name",
                Phone = "987654321",
                Image = "admin-image.jpg",
                Dob = new DateTime(1985, 5, 15),
                Role = User_Role.ADMIN
            };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ReturnsAsync(userLoginDto);
            _jwtHelperMock.Setup(x => x.GenerateJwtToken(userLoginDto)).Returns("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as Repository.Entities.ApiResponse;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Success);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Authentication success.", response.Message);

            var data = response.Data as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.Token);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.UserInfo);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Admin Name", data.UserInfo.Name);
        }

        [TestMethod]
        public async Task Login_ShouldReturnOkResult_WhenValidCredentialsForCustomer()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "customer@example.com", Password = "validPassword" };
            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = "customer@example.com",
                Role = User_Role.CUSTOMER,
                Name = "Customer Name",
                Phone = "123456789",
                Image = "customer-image.jpg",
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male"
            };

            var customerInfo = new CustomerInfoResponse
            {
                Uid = 1,
                Email = "customer@example.com",
                Name = "Customer Name",
                Phone = "123456789",
                Image = "customer-image.jpg",
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male",
                Role = User_Role.CUSTOMER
            };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ReturnsAsync(userLoginDto);
            _jwtHelperMock.Setup(x => x.GenerateJwtToken(userLoginDto)).Returns("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as Repository.Entities.ApiResponse;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Success);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Authentication success.", response.Message);

            var data = response.Data as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.Token);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(data.UserInfo);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Customer Name", data.UserInfo.Name);
        }

        [TestMethod]
        public async Task Login_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "invalid@example.com", Password = "wrongPassword" };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);

            var response = badRequestResult.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Dữ liệu không hợp lệ.", response.message);
        }

        [TestMethod]
        public async Task Login_ShouldReturnUnauthorized_WhenUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "admin@example.com", Password = "invalidPassword" };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(unauthorizedResult);
        Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(401, unauthorizedResult.StatusCode);

            var response = unauthorizedResult.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Unauthorized", response.message);
        }

        [TestMethod]
        public async Task Login_ShouldReturnBadRequest_WhenEmailHasInvalidFormat()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "invalidEmailFormat", Password = "validPassword" };

            // Mock the behavior of the service (optional)
            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);

            var response = badRequestResult.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Invalid email format.", response.message);
        }
    }
}
