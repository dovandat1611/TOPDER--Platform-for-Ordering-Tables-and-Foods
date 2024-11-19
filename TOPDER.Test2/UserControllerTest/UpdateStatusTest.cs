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
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class UpdateStatusTest
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
        public async Task UpdateStatus_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            string status = Common_Status.ACTIVE;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null); // User không tồn tại

            // Act
            var result = await _controller.UpdateStatus(userId, status);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Người dùng không tồn tại.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string invalidStatus = "UNKNOWN_STATUS";

            // Mô phỏng việc người dùng tồn tại
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { Uid = userId, Status = Common_Status.ACTIVE });

            // Act
            var result = await _controller.UpdateStatus(userId, invalidStatus);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Trạng thái không hợp lệ. Vui lòng chọn ACTIVE hoặc INACTIVE.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_StatusAlreadyMatches_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            string status = Common_Status.ACTIVE;

            // Mô phỏng việc người dùng đã có trạng thái ACTIVE
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { Uid = userId, Status = status });

            // Act
            var result = await _controller.UpdateStatus(userId, status);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Người dùng đã có trạng thái này.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            string newStatus = Common_Status.INACTIVE;

            // Mô phỏng việc người dùng tồn tại và có trạng thái ACTIVE
            var user = new User { Uid = userId, Status = Common_Status.ACTIVE };
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(true); // Cập nhật thành công

            // Act
            var result = await _controller.UpdateStatus(userId, newStatus);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái người dùng thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_UpdateFailed_ReturnsBadRequest()
        {
            // Arrange
            int userId = 1;
            string newStatus = Common_Status.INACTIVE;

            // Mô phỏng việc người dùng tồn tại và có trạng thái ACTIVE
            var user = new User { Uid = userId, Status = Common_Status.ACTIVE };
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(false); // Cập nhật thất bại

            // Act
            var result = await _controller.UpdateStatus(userId, newStatus);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái người dùng thất bại.", badRequestResult.Value);
        }
    }

}

