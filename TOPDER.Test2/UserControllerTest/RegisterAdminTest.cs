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
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class RegisterAdminTest
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
        public async Task RegisterAdmin_Success_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDto()
            {
                Uid = 1,
                Email = "topder.vn@gmail.com",
                RoleId = 1, // Admin
                Password = BCrypt.Net.BCrypt.HashPassword("Topder123"),
                OtpCode = string.Empty,
                IsVerify = true,
                Status = Common_Status.ACTIVE,
                IsExternalLogin = false,
                CreatedAt = DateTime.Now
            };

            var walletBalanceDto = new WalletBalanceDto()
            {
                WalletId = 0,
                Uid = 1,
                WalletBalance = 0
            };

            var adminDto = new AdminDto()
            {
                Uid = 1,
                Name = "TOPDER",
                Phone = "0968519615",
                Dob = DateTime.Now,
                Image = Default_Avatar.ADMIN
            };

            // Setup mock returns
            _userServiceMock.Setup(x => x.AddAsync(It.IsAny<UserDto>())).ReturnsAsync(new User { Uid = 1, Email = userDto.Email });
            _adminServiceMock.Setup(x => x.AddAsync(It.IsAny<AdminDto>())).ReturnsAsync(new Admin { Uid = 1 });
            _sendMailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterAdmin();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}

