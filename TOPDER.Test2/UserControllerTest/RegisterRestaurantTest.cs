using CloudinaryDotNet.Actions;
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
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class RegisterRestaurantTest
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
        public async Task RegisterRestaurant_ReturnsOk_WhenRequestIsValid()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("dummy file content"));
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.Length).Returns(1024); // 1 KB
            mockFile.Setup(f => f.FileName).Returns("logo.png");

            var createRestaurantRequest = new CreateRestaurantRequest
            {
                CategoryRestaurantId = 1,
                NameOwner = "Owner Name",
                NameRes = "Restaurant Name",
                File = mockFile.Object,
                OpenTime = new TimeSpan(9, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                Address = "123 Street",
                Phone = "1234567890",
                Email = "owner@example.com",
                Price = 100.00m,
                MaxCapacity = 50,
                Password = "password123"
            };

            _cloudinaryServiceMock
                .Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new ImageUploadResult
                {
                    SecureUrl = new Uri("https://example.com/image.jpg")
                });

            _userServiceMock
                .Setup(service => service.AddAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(new User
                {
                    Uid = 1,
                    Email = createRestaurantRequest.Email,
                    RoleId = 2,
                    Password = BCrypt.Net.BCrypt.HashPassword(createRestaurantRequest.Password),
                    IsVerify = true,
                    Status = Common_Status.INACTIVE,
                    CreatedAt = DateTime.Now
                });

            _walletServiceMock
                .Setup(service => service.AddWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                .ReturnsAsync(true);

            _restaurantServiceMock
                .Setup(service => service.AddAsync(It.IsAny<CreateRestaurantRequest>()))
                .ReturnsAsync(new Restaurant { Uid = 1, NameRes = createRestaurantRequest.NameRes });

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterRestaurant(createRestaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }


        [TestMethod]
        public async Task RegisterRestaurant_NoFileUploaded_ReturnsBadRequest()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();
            restaurantRequest.File = null;

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No file was uploaded.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task RegisterRestaurant_ImageUploadFailed_Returns400()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();

            _cloudinaryServiceMock
                .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((ImageUploadResult)null);

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No file was uploaded.", objectResult.Value);
        }

        [TestMethod]
        public async Task RegisterRestaurant_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();
            _controller.ModelState.AddModelError("Email", "Email is required.");

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task RegisterRestaurant_UserCreationFailed_ReturnsBadRequest()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();
            _userServiceMock
             .Setup(s => s.AddAsync(It.IsAny<UserDto>()))
             .ReturnsAsync(new User { Email = restaurantRequest.Email });

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
                   Microsoft.VisualStudio.TestTools.UnitTesting.    Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No file was uploaded.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task RegisterRestaurant_WalletCreationFailed_ReturnsBadRequest()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();
            var uploadedImage = new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.OK, SecureUrl = new Uri("http://example.com/image1.jpg") };

            _cloudinaryServiceMock
                .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadedImage);

            _userServiceMock
                .Setup(s => s.AddAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(new User { Email = restaurantRequest.Email });

            _walletServiceMock
                .Setup(s => s.AddWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
        }

        private CreateRestaurantRequest GetValidRestaurantRequest()
        {
            return new CreateRestaurantRequest
            {
                File = new Mock<IFormFile>().Object,
                Email = "test@example.com",
                Password = "Password123!",
                NameRes = "Test Restaurant",
                CategoryRestaurantId = 1,
                NameOwner = "Owner Name",
                Address = "Test Address",
                Phone = "123456789",
                Price = 100.00m,
                MaxCapacity = 50,
                OpenTime = new TimeSpan(8, 0, 0),
                CloseTime = new TimeSpan(20, 0, 0)
            };
        }
    }
}
