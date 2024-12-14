using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryRestaurantControllerTest
{
    [TestClass]
    public class CategoryExistTest
    {
        private Mock<ICategoryRestaurantService> _mockCategoryRestaurantService;
        private CategoryRestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRestaurantService = new Mock<ICategoryRestaurantService>();
            _controller = new CategoryRestaurantController(_mockCategoryRestaurantService.Object);
        }

        [TestMethod]
        public async Task CategoryExist_WhenCategoriesExist_ReturnsOk()
        {
            // Arrange
            var mockCategories = new List<CategoryRestaurantDto>
        {
            new CategoryRestaurantDto { CategoryRestaurantId = 1, CategoryRestaurantName = "Italian Cuisine" },
            new CategoryRestaurantDto { CategoryRestaurantId = 2, CategoryRestaurantName = "Mexican Cuisine" }
        };

            _mockCategoryRestaurantService.Setup(service => service.CategoryExistAsync())
                .ReturnsAsync(mockCategories); // Simulate categories found

            // Act
            var result = await _controller.CategoryExist() as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var categories = result.Value as List<CategoryRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(categories);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, categories.Count); // Check if we have two categories
        }

        [TestMethod]
        public async Task CategoryExist_WhenNoCategoriesExist_ReturnsNotFound()
        {
            // Arrange
            _mockCategoryRestaurantService.Setup(service => service.CategoryExistAsync())
                .ReturnsAsync(new List<CategoryRestaurantDto>()); // Simulate no categories found

            // Act
            var result = await _controller.CategoryExist() as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No categories found that are associated with any restaurant.", result.Value);
        }

        [TestMethod]
        public async Task CategoryExist_WhenExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var exceptionMessage = "An error occurred while fetching categories.";
            _mockCategoryRestaurantService.Setup(service => service.CategoryExistAsync())
                .ThrowsAsync(new ApplicationException(exceptionMessage)); // Simulate exception

            // Act
            var result = await _controller.CategoryExist() as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(exceptionMessage, result.Value);
        }

        [TestMethod]
        public async Task CategoryExist_WhenUnexpectedException_ReturnsInternalServerError()
        {
            // Arrange
            _mockCategoryRestaurantService.Setup(service => service.CategoryExistAsync())
                .ThrowsAsync(new Exception("Unexpected error")); // Simulate unexpected exception

            // Act
            var result = await _controller.CategoryExist() as ObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while processing your request: Unexpected error", result.Value);
        }
    }
}
