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
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class ForgotPasswordTest
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
        public async Task ForgotPassword_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var email = "test@example.com";
            _userServiceMock.Setup(service => service.GetUserByEmail(email))
                .ReturnsAsync((UserLoginDTO)null); // Returns null for UserLoginDTO if no user is found

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy tài khoản với email: test@example.com", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ForgotPassword_CannotCreateOTP_ReturnsBadRequest()
        {
            // Arrange
            var email = "test@example.com";
            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = email,
                Role = User_Role.CUSTOMER
            };
            _userServiceMock.Setup(service => service.GetUserByEmail(email)).ReturnsAsync(userLoginDto);
            _userOtpRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<UserOtp>())).ReturnsAsync(false);

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual("Không thể tạo OTP", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ForgotPassword_Success_ReturnsOk()
        {
            // Arrange
            var email = "test@example.com";
            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = email,
                Role = User_Role.CUSTOMER
            };
            _userServiceMock.Setup(service => service.GetUserByEmail(email)).ReturnsAsync(userLoginDto);

            var otpCode = "123456";
            var userOtp = new UserOtp
            {
                Uid = userLoginDto.Uid,
                OtpCode = otpCode,
                ExpiresAt = DateTime.Now.AddMinutes(5),
                CreatedAt = DateTime.Now,
                IsUse = false
            };

            _userOtpRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<UserOtp>())).ReturnsAsync(true);
            _sendMailServiceMock.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForgotPassword(email);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("OTP đã được gửi đến email của bạn.", okResult.Value);
        }
    }
}
