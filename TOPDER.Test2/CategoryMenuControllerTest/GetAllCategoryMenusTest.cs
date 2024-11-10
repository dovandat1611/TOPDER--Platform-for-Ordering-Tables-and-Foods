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
    public class GetAllCategoryMenusTest
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
        public async Task GetAllCategoryMenus_WithRestaurantIdZero_ReturnsOk()
        {
            // Arrange
            int restaurantId = 0; // Assuming 0 is treated as valid for this test case
            var mockCategoryMenus = new List<CategoryMenuDto>(); // Simulating an empty list of category menus
            _mockCategoryMenuService.Setup(service => service.GetAllCategoryMenuAsync(restaurantId))
                .ReturnsAsync(mockCategoryMenus);

            // Act
            var result = await _controller.GetAllCategoryMenus(restaurantId) as OkObjectResult;
            var categoryMenus = result?.Value as List<CategoryMenuDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(categoryMenus);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, categoryMenus.Count); // Expecting an empty list
        }

        [TestMethod]
        public async Task GetAllCategoryMenus_WithRestaurantIdMinusOne_ReturnsOk()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId but treated as valid for this test case
            var mockCategoryMenus = new List<CategoryMenuDto>(); // Simulating an empty list of category menus
            _mockCategoryMenuService.Setup(service => service.GetAllCategoryMenuAsync(restaurantId))
                .ReturnsAsync(mockCategoryMenus);

            // Act
            var result = await _controller.GetAllCategoryMenus(restaurantId) as OkObjectResult;
            var categoryMenus = result?.Value as List<CategoryMenuDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(categoryMenus);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, categoryMenus.Count); // Expecting an empty list
        }


        [TestMethod]
        public async Task GetAllCategoryMenus_WithRestaurantIdOne_ReturnsCategoryMenus()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            var mockCategoryMenus = new List<CategoryMenuDto>
        {
            new CategoryMenuDto { CategoryMenuId = 1, CategoryMenuName = "Appetizers" },
            new CategoryMenuDto { CategoryMenuId = 2, CategoryMenuName = "Main Course" }
        };
            _mockCategoryMenuService.Setup(service => service.GetAllCategoryMenuAsync(restaurantId))
                .ReturnsAsync(mockCategoryMenus);

            // Act
            var result = await _controller.GetAllCategoryMenus(restaurantId) as OkObjectResult;
            var categoryMenus = result?.Value as List<CategoryMenuDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(categoryMenus);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, categoryMenus.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Appetizers", categoryMenus[0].CategoryMenuName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Main Course", categoryMenus[1].CategoryMenuName);
        }

        
    }
}
