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
    public class ResetPasswordTest
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
        public async Task ResetPassword_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new ResetPasswordRequest { Email = "test@example.com", NewPassword = "NewPassword123" };
            _userRepositoryMock.Setup(repo => repo.QueryableAsync()).ReturnsAsync(Enumerable.Empty<User>().AsQueryable());

            // Act
            var result = await _controller.ResetPassword(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy tài khoản với email: {request.Email}", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ResetPassword_Success_ReturnsOk()
        {
            // Arrange
            var request = new ResetPasswordRequest { Email = "test@example.com", NewPassword = "NewPassword123" };
            var user = new User { Email = request.Email, Password = "OldPassword123" };
            // Mock the repository to return an empty IQueryable<User>
            _userRepositoryMock.Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(Enumerable.Empty<User>().AsQueryable()); // Convert IEnumerable to IQueryable            _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _controller.ResetPassword(request);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mật khẩu đã được đặt lại thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task ResetPassword_FailToUpdate_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ResetPasswordRequest { Email = "test@example.com", NewPassword = "NewPassword123" };
            var user = new User { Email = request.Email, Password = "OldPassword123" };

            // Mock the repository to return a user when querying
            _userRepositoryMock.Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(new List<User> { user }.AsQueryable()); // Return a user with matching email

            // Mock the UpdateAsync to return false, simulating a failure
            _userRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(false); // Simulate failure of password update

            // Act
            var result = await _controller.ResetPassword(request);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}
