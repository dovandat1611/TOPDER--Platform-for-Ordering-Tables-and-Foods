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
            
            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_ShouldReturnOkResult_WhenValidCredentialsForAdmin()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "admin@example.com", Password = "wrongPassword" };
            
            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_ShouldReturnOkResult_WhenValidCredentialsForCustomer()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "customer@example.com", Password = "validPassword" };
           
            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

        }

        [TestMethod]
        public async Task Login_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "invalid@example.com", Password = "wrongPassword" };

            _userServiceMock.Setup(x => x.GetUserByEmailAndPassword(loginModel)).ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(unauthorizedResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(401, unauthorizedResult.StatusCode);

            var response = unauthorizedResult.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
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
        }

    }
}
