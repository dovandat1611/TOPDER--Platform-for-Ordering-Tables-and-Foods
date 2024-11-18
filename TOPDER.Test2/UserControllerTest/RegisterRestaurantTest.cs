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
        public async Task RegisterRestaurant_Successful_ReturnsOk()
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
                .ReturnsAsync(true);
            _restaurantServiceMock
                .Setup(s => s.AddAsync(It.IsAny<CreateRestaurantRequest>()))
                .ReturnsAsync(new Restaurant { NameRes = restaurantRequest.NameRes });

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult.Value);
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
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Image upload failed.", objectResult.Value);
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
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tạo được Ví cho người dùng.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task RegisterRestaurant_UnexpectedException_Returns500()
        {
            // Arrange
            var restaurantRequest = GetValidRestaurantRequest();
            _cloudinaryServiceMock
                .Setup(s => s.UploadImageAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.RegisterRestaurant(restaurantRequest);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(objectResult.Value.ToString().Contains("Failed to create restaurant: Unexpected error"));
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
