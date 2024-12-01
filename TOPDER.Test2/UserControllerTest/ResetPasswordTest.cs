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
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = "test@example.com",
                NewPassword = "NewSecurePassword123!"
            };

            var mockUser = new User
            {
                Uid = 1,
                Email = resetPasswordRequest.Email,
                Password = "OldHashedPassword"
            };

            var userQuery = new List<User> { mockUser }.AsQueryable();

            _userRepositoryMock
                .Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(userQuery);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ResetPassword(resetPasswordRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mật khẩu đã được đặt lại thành công.", okResult.Value);

            // Verify password hashing and update
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<User>(u =>
                u.Email == resetPasswordRequest.Email &&
                u.Password != "OldHashedPassword"  // Make sure the password has changed
            )), Times.Once);
        }

        [TestMethod]
        public async Task ResetPassword_UpdateFails_ReturnsServerError()
        {
            // Arrange
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = "test@example.com",
                NewPassword = "NewSecurePassword123!"
            };

            var mockUser = new User
            {
                Uid = 1,
                Email = resetPasswordRequest.Email,
                Password = "OldHashedPassword"
            };

            var userQuery = new List<User> { mockUser }.AsQueryable();

            _userRepositoryMock
                .Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(userQuery);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ResetPassword(resetPasswordRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var serverErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Lỗi khi cập nhật mật khẩu.", serverErrorResult.Value);
        }

    }
}
