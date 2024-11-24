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
        public async Task Create_DishNameIsNull_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = null, // Invalid vì DishName là bắt buộc
                Price = 100,
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            _controller.ModelState.AddModelError("DishName", "Dish name is required");

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }
        [TestMethod]
        public async Task Create_PriceIsZero_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Special Dish",
                Price = 0, // Invalid vì giá món ăn không thể bằng 0
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            _controller.ModelState.AddModelError("Price", "Price must be greater than zero");

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }

        [TestMethod]
        public async Task Create_RestaurantIdIsZero_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 0, // Invalid vì RestaurantId là bắt buộc
                CategoryMenuId = 1,
                DishName = "Special Dish",
                Price = 100,
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            _controller.ModelState.AddModelError("RestaurantId", "Restaurant ID is required");

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }
        [TestMethod]
        public async Task Create_ValidMenuDto_ReturnsOkResult()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Special Dish",
                Price = 100,
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            var expectedResponse = new { Success = true, MenuId = 1 };
            _menuServiceMock
                .Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task Create_CategoryMenuIdIsNull_ReturnsBadRequest()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = null, // Invalid nếu CategoryMenuId là bắt buộc
                DishName = "Special Dish",
                Price = 100,
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            _controller.ModelState.AddModelError("CategoryMenuId", "Category menu ID is required");

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }
        [TestMethod]
        public async Task Create_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var menuDto = new MenuDto
            {
                MenuId = 1,
                RestaurantId = 1,
                CategoryMenuId = 1,
                DishName = "Special Dish",
                Price = 100,
                Image = "dish.jpg",
                Description = "Delicious dish"
            };

            _menuServiceMock
                .Setup(service => service.AddAsync(It.IsAny<MenuDto>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Create(menuDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(objectResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create restaurant menu: Test exception", objectResult.Value);
        }

    }
}
