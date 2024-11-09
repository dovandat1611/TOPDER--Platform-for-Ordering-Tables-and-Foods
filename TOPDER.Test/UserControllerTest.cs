using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TOPDER.Test
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public async Task Login_WhenModelIsInvalid_ReturnBadRequest()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockJwtHelper = new Mock<JwtHelper>();
            var controller = new UserController(mockUserService.Object, mockJwtHelper.Object);

            var invalidLoginModel = new LoginModel
            {
                Email = "", // Email không hợp lệ
                Password = "password123"
            };

            controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await controller.Login(invalidLoginModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Dữ liệu không hợp lệ.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_WhenCredentialsAreValid_ReturnsTokenAndUserInfo()
        {
            // Arrange
            var validLoginModel = new LoginModel
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var userLoginDto = new UserLoginDTO
            {
                Uid = 1,
                Email = "user@example.com",
                Role = User_Role.CUSTOMER,
                Name = "John Doe",
                Phone = "123456789",
                Image = "image_url",
                Dob = DateTime.Now.AddYears(-25),
                Gender = "Male"
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserByEmailAndPassword(validLoginModel))
                .ReturnsAsync(userLoginDto);

            var mockJwtHelper = new Mock<JwtHelper>();
            mockJwtHelper.Setup(helper => helper.GenerateJwtToken(userLoginDto))
                .Returns("jwt_token");

            var controller = new UserController(mockUserService.Object, mockJwtHelper.Object);

            // Act
            var result = await controller.Login(validLoginModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var apiResponse = okResult.Value as Repository.Entities.ApiResponse;
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Authentication success.", apiResponse.Message);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("jwt_token", apiResponse.Data.Token);
        }

        [TestMethod]
        public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var invalidLoginModel = new LoginModel
            {
                Email = "wrongemail@example.com",
                Password = "wrongpassword"
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUserByEmailAndPassword(invalidLoginModel))
                .ThrowsAsync(new UnauthorizedAccessException("Email hoặc mật khẩu không hợp lệ."));

            var mockJwtHelper = new Mock<JwtHelper>();
            var controller = new UserController(mockUserService.Object, mockJwtHelper.Object);

            // Act
            var result = await controller.Login(invalidLoginModel);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            var response = unauthorizedResult.Value as dynamic;
            Assert.AreEqual("Email hoặc mật khẩu không hợp lệ.", response.message);
        }

    }
}
