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
    public class CreateTest
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
        public async Task CreateCategoryRestaurant_WithValidData_ReturnsOk()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = 1,
                CategoryRestaurantName = "Italian Cuisine"
            };

            _mockCategoryRestaurantService.Setup(service => service.AddAsync(categoryRestaurantDto))
                .ReturnsAsync(true); // Simulate success

            // Act
            var result = await _controller.Create(categoryRestaurantDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task CreateCategoryRestaurant_WithServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = -1,
                CategoryRestaurantName = "Italian Cuisine"
            };

            _mockCategoryRestaurantService.Setup(service => service.AddAsync(categoryRestaurantDto))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.Create(categoryRestaurantDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task CreateCategoryRestaurant_WithNullCategoryRestaurantName_ReturnsOk()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = 1,
                CategoryRestaurantName = null // Invalid CategoryRestaurantName
            };

            _mockCategoryRestaurantService.Setup(service => service.AddAsync(categoryRestaurantDto))
                .ReturnsAsync(true); // Simulate success despite null name (business rule could check this)

            // Act
            var result = await _controller.Create(categoryRestaurantDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

    }
}
