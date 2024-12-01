using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit.Cryptography;
using Moq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class CheckAccessTokenTest
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
        public async Task CheckAccessToken_ValidToken_ReturnsOk()
        {
            // Arrange
            string accessToken = "validAccessToken";
            var expectedResult = new Repository.Entities.ApiResponse
            {
                Success = true,
                Data = "dfuhdfdkxvxhcvicxcuxivhsdivvncxvjxvxchxuicvhu",
                Message = "Authentication success"
            };
            _identityServiceMock
                .Setup(service => service.AuthenticateWithGoogle(accessToken))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CheckAccessToken(accessToken);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedResult, okResult.Value);
        }

        [TestMethod]
        public async Task CheckAccessToken_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            string accessToken = "invalidAccessToken";
            var expectedResult = new Repository.Entities.ApiResponse
            {
                Success = false,
                Message = "Invalid Google token."
            };
            _identityServiceMock
                .Setup(service => service.AuthenticateWithGoogle(accessToken))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CheckAccessToken(accessToken);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedResult.Message, badRequestResult.Value);
        }

        [TestMethod]
        public async Task CheckAccessToken_ServiceThrowsException_Returns500()
        {
            // Arrange
            string accessToken = "anyAccessToken";
            _identityServiceMock
                .Setup(service => service.AuthenticateWithGoogle(accessToken))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            IActionResult result;
            try
            {
                result = await _controller.CheckAccessToken(accessToken);
            }
            catch (Exception ex)
            {
                // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Service error", ex.Message);
                return;
            }

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Expected exception was not thrown.");
        }
    }
}
