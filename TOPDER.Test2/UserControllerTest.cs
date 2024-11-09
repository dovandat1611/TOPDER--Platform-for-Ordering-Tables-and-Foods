using Castle.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2
{
    [TestClass]
    public class UserControllerTests
    {
        private readonly IConfiguration _config;
        private UserController _controller;
        private Mock<IUserService> _mockUserService;
        private Mock<JwtHelper> _mockJwtHelper;
        private Mock<IRestaurantService> _mockRestaurantService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private Mock<ISendMailService> _mockSendMailService;
        private Mock<IWalletService> _mockWalletService;
        private Mock<IAdminService> _mockAdminService;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<IUserOtpRepository> _mockUserOtpRepository;
        private Mock<JwtHelper> _mockJwtHelpers;

        // Phương thức khởi tạo controller
        private void InitializeController()
        {
            _mockUserService = new Mock<IUserService>();
            _mockRestaurantService = new Mock<IRestaurantService>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _mockSendMailService = new Mock<ISendMailService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockAdminService = new Mock<IAdminService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUserOtpRepository = new Mock<IUserOtpRepository>();
            _mockJwtHelpers = new Mock<JwtHelper>(_config);

            _controller = new UserController(
                _mockRestaurantService.Object,
                _mockCloudinaryService.Object,
                _mockSendMailService.Object,
                _mockUserService.Object,
                _mockCustomerService.Object,
                _mockWalletService.Object,
                _mockJwtHelpers.Object,
                _mockAdminService.Object,
                _mockUserRepository.Object,
                _mockIdentityService.Object,
                _mockUserOtpRepository.Object
            );
        }
        [TestMethod]
        public async Task Login_WhenModelIsInvalid_ReturnBadRequest()
        {
            // Arrange
            InitializeController();  // Khởi tạo controller

            var invalidLoginModel = new LoginModel
            {
                Email = "", // Email không hợp lệ
                Password = "password123"
            };

            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(invalidLoginModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Dữ liệu không hợp lệ.", badRequestResult.Value);
        }
        [TestMethod]
        public async Task Login_WhenSuccessful_ReturnOk()
        {
            // Arrange
            InitializeController();  // Khởi tạo controller

            var validLoginModel = new LoginModel
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var mockUserDto = new UserLoginDTO
            {
                Uid = 2,
                Email = "datdvhe161664@fpt.edu.vn",
                Role = User_Role.CUSTOMER,
                Name = "John Doe",
                Phone = "123456789",
            };

            _mockUserService.Setup(x => x.GetUserByEmailAndPassword(validLoginModel))
                .ReturnsAsync(mockUserDto);

            _mockJwtHelper.Setup(x => x.GenerateJwtToken(mockUserDto))
                .Returns("sampleJwtToken");

            // Act
            var result = await _controller.Login(validLoginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as Repository.Entities.ApiResponse;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Success);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Authentication success.", response.Message);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response.Data);
        }

        [TestMethod]
        public async Task Login_WhenUserNotFound_ReturnUnauthorized()
        {
            // Arrange
            InitializeController();  // Khởi tạo controller

            var invalidLoginModel = new LoginModel
            {
                Email = "nonexistentemail@example.com",
                Password = "password123"
            };

            _mockUserService.Setup(x => x.GetUserByEmailAndPassword(It.IsAny<LoginModel>()))
                .ThrowsAsync(new UnauthorizedAccessException("Email hoặc mật khẩu không hợp lệ."));

            // Act
            var result = await _controller.Login(invalidLoginModel);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(unauthorizedResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Email hoặc mật khẩu không hợp lệ.", unauthorizedResult.Value);
        }

    }



}

