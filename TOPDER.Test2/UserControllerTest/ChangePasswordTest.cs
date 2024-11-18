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

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class ChangePasswordTest
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
        public async Task ChangePassword_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                OldPassword = "oldPassword123",
                NewPassword = "newPassword123"
            };

            // Simulate an invalid model state
            _controller.ModelState.AddModelError("OldPassword", "OldPassword is required");

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Dữ liệu không hợp lệ.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangePassword_FailToChangePassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                OldPassword = "wrongOldPassword123",
                NewPassword = "newPassword123"
            };

            // Mock the IUserService to return false (indicating failure to change the password)
            _userServiceMock.Setup(service => service.ChangePassword(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mật khẩu cũ không đúng hoặc không tìm thấy tài khoản.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangePassword_Success_ReturnsOk()
        {
            // Arrange
            var request = new ChangePasswordRequest
            {
                OldPassword = "oldPassword123",
                NewPassword = "newPassword123"
            };

            // Mock the IUserService to return true (indicating success)
            _userServiceMock.Setup(service => service.ChangePassword(It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(request);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mật khẩu đã được thay đổi thành công.", okResult.Value);
        }
    }
}
