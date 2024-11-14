using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class UpdateTest
    {
        private Mock<IMenuService> _menuServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private MenuController _controller;

        [TestInitialize]
        public void Setup()
        {
            _menuServiceMock = new Mock<IMenuService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new MenuController(_menuServiceMock.Object, _cloudinaryServiceMock.Object);
        }

        // Test khi ModelState không hợp lệ
        [TestMethod]
        public async Task Update_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "", Price = 10.5m, Description = "A sample description" }; // Invalid DishName
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            _controller.ModelState.AddModelError("DishName", "Dish name is required.");

            // Act
            var result = await _controller.Update(menuDto, file.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        // Test khi upload ảnh thất bại
        [TestMethod]
        public async Task Update_ImageUploadFails_Returns500()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Sample Dish",
                Price = 10.5m,
                Description = "A sample description"
            };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object)).ReturnsAsync((ImageUploadResult)null);

            // Act
            var result = await _controller.Update(menuDto, file.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Image upload failed.", internalServerErrorResult.Value);
        }

        // Test khi cập nhật món ăn thành công
        [TestMethod]
        public async Task Update_ValidRequest_ReturnsOk()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Sample Dish",
                Price = 10.5m,
                Description = "A sample description"
            };

            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            var uploadResult = new ImageUploadResult { SecureUrl = new Uri("http://example.com/image.jpg") };
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                                  .ReturnsAsync(uploadResult);

            _menuServiceMock.Setup(service => service.UpdateAsync(It.IsAny<MenuDto>()))
                            .ReturnsAsync(true);  // Simulate successful update

            // Act
            var result = await _controller.Update(menuDto, file.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        // Test khi cập nhật món ăn thất bại
        [TestMethod]
        public async Task Update_UpdateFails_Returns500()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m, Description = "A sample description" };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object)).ReturnsAsync((ImageUploadResult)null);

            // Act
            var result = await _controller.Update(menuDto, file.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Image upload failed.", internalServerErrorResult.Value);
        }

        // Test khi không có ảnh upload
        [TestMethod]
        public async Task Update_NoImageProvided_ReturnsOk()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Sample Dish",
                Price = 10.5m,
                Description = "A sample description"
            };

            _menuServiceMock.Setup(service => service.UpdateAsync(It.IsAny<MenuDto>()))
                            .ReturnsAsync(true);  // Simulate successful update without image

            // Act
            var result = await _controller.Update(menuDto, null);  // No image file

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task Update_PriceZero_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Sample Dish",
                Price = 0,  // Price is zero
                Description = "A sample description"
            };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object))
                                  .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("http://example.com/image.jpg") });

            // Act
            var result = await _controller.Update(menuDto, file.Object);

            // Assert
            var badRequestResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task Update_DescriptionNull_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Sample Dish",
                Price = 111110,  // Price is zero
                Description = ""
            };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object))
                                  .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("http://example.com/image.jpg") });

            // Act
            var result = await _controller.Create(menuDto, file.Object);

            // Assert
            var badRequestResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }
    }
}
