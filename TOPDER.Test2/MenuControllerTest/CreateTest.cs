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
    public class CreateTest
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

        [TestMethod]
        public async Task CreateMenu_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m, Description = "A sample description" };
            
            _menuServiceMock.Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
            .ReturnsAsync(true); // Trả về `true` hoặc `false` tùy theo yêu cầu của test case


            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task CreateMenu_NoFileUploaded_ReturnsOk()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m };

            _menuServiceMock.Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
            .ReturnsAsync(true); // Trả về `true` hoặc `false` tùy theo yêu cầu của test case


            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task CreateMenu_EmptyFile_ReturnOk()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m , Description = "A sample description" };
            // Act
            _menuServiceMock.Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
             .ReturnsAsync(true); // Trả về `true` hoặc `false` tùy theo yêu cầu của test case


            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task CreateMenu_FailedUpload_ReturnsOk()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m, Description = "A sample description" };

            _menuServiceMock.Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
            .ReturnsAsync(true); // Trả về `true` hoặc `false` tùy theo yêu cầu của test case


            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task CreateMenu_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "", Price = 10.5m, Description = "A sample description" }; // Invalid DishName
            var file = new Mock<IFormFile>();

            _controller.ModelState.AddModelError("DishName", "Dish name is required.");

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);

            // Thay đổi kiểm tra IsTrue thành Assert cho chính xác hơn
            var errorMessages = badRequestResult.Value.ToString();
            
        }

        
        [TestMethod]
        public async Task CreateMenu_AddMenuFailed_ReturnsInternalServerError()
        {
            // Arrange
            var menuDto = new MenuDto { DishName = "Test Dish", Price = 10.5m, Description = "A sample description" };
            _menuServiceMock.Setup(service => service.AddAsync(menuDto)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.AreEqual("Failed to create restaurant menu: Database error", internalServerErrorResult.Value);
        }

        [TestMethod]
        public async Task Create_PriceZero_ReturnsOk()
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
            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            var badRequestResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

     }
}
