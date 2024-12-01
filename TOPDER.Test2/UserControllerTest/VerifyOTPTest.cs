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
    public class VerifyOTPTest
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
        public async Task VerifyOTP_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new VerifyOtpRequest { Email = "test@example.com", Otp = "123456" };
            _userServiceMock.Setup(service => service.GetUserByEmail(request.Email)).ReturnsAsync((UserLoginDTO)null);

            // Act
            var result = await _controller.VerifyOTP(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy tài khoản với email: {request.Email}", badRequestResult.Value);
        }

        [TestMethod]
        public async Task VerifyOTP_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            var request = new VerifyOtpRequest { Email = "test@example.com", Otp = "123456" };
            var user = new UserLoginDTO { Uid = 1, Email = request.Email };
            _userServiceMock.Setup(service => service.GetUserByEmail(request.Email)).ReturnsAsync(user);
            _userOtpRepositoryMock.Setup(repo => repo.GetValidOtpAsync(user.Uid, request.Otp)).ReturnsAsync((UserOtp)null);

            // Act
            var result = await _controller.VerifyOTP(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không thấy OTP nào hợp lệ hoặc OTP đã hết hạn. Hãy thử tạo lại OTP (Thời gian hiệu lực là 5 phút).", badRequestResult.Value);
        }

        [TestMethod]
        public async Task VerifyOTP_Success_ReturnsOk()
        {
            // Arrange
            var request = new VerifyOtpRequest { Email = "test@example.com", Otp = "123456" };
            var user = new UserLoginDTO { Uid = 1, Email = request.Email };
            var userOtp = new UserOtp { OtpCode = "123456", IsUse = false, ExpiresAt = DateTime.Now.AddMinutes(5) };
            _userServiceMock.Setup(service => service.GetUserByEmail(request.Email)).ReturnsAsync(user);
            _userOtpRepositoryMock.Setup(repo => repo.GetValidOtpAsync(user.Uid, request.Otp)).ReturnsAsync(userOtp);
            _userOtpRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<UserOtp>())).ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyOTP(request);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("OTP đã được xác thực thành công.", okResult.Value);
        }

    }
}
