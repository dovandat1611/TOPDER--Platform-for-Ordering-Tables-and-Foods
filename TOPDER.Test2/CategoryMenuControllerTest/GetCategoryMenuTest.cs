using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryMenuControllerTest
{
    [TestClass]
    public class GetCategoryMenuTest
    {
        private Mock<ICategoryMenuService> _mockCategoryMenuService;
        private CategoryMenuController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryMenuService = new Mock<ICategoryMenuService>();
            _controller = new CategoryMenuController(_mockCategoryMenuService.Object);
        }

        [TestMethod]
        public async Task GetCategoryMenu_WithRestaurantIdNegative_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId
            int categoryMenuId = 1;

            // Act
            var result = await _controller.GetCategoryMenu(restaurantId, categoryMenuId) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant ID must be greater than zero.", result.Value);
        }

        [TestMethod]
        public async Task GetCategoryMenu_WithValidRestaurantIdAndCategoryMenuId_ReturnsCategoryMenu()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            int categoryMenuId = 1; // Valid categoryMenuId
            var mockCategoryMenu = new CategoryMenuDto { CategoryMenuId = categoryMenuId, CategoryMenuName = "Main Course" };
            _mockCategoryMenuService.Setup(service => service.GetItemAsync(categoryMenuId, restaurantId))
                .ReturnsAsync(mockCategoryMenu);

            // Act
            var result = await _controller.GetCategoryMenu(restaurantId, categoryMenuId) as OkObjectResult;
            var categoryMenu = result?.Value as CategoryMenuDto;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(categoryMenu);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(categoryMenuId, categoryMenu.CategoryMenuId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Main Course", categoryMenu.CategoryMenuName);
        }

        [TestMethod]
        public async Task GetCategoryMenu_WithValidRestaurantIdAndInvalidCategoryMenuId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            int categoryMenuId = -1; // Invalid categoryMenuId
            _mockCategoryMenuService.Setup(service => service.GetItemAsync(categoryMenuId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetCategoryMenu(restaurantId, categoryMenuId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Menu với ID {categoryMenuId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task GetCategoryMenu_WithValidRestaurantIdAndZeroCategoryMenuId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            int categoryMenuId = 0; // Invalid categoryMenuId
            _mockCategoryMenuService.Setup(service => service.GetItemAsync(categoryMenuId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetCategoryMenu(restaurantId, categoryMenuId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Menu với ID {categoryMenuId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task GetCategoryMenu_WithInvalidRestaurantIdAndValidCategoryMenuId_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId
            int categoryMenuId = 1; // Valid categoryMenuId
                                    // No need to mock service call, as we expect the method to return BadRequest based on restaurantId validation

            // Act
            var result = await _controller.GetCategoryMenu(restaurantId, categoryMenuId) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant ID must be greater than zero.", result.Value);
        }

    }
}
