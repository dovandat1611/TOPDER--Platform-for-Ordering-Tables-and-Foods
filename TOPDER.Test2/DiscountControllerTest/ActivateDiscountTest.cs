using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.DiscountControllerTest
{
    [TestClass]
    public class ActivateDiscountTest
    {
        private DiscountController _controller;
        private Mock<IDiscountService> _mockDiscountService;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        // Test case for valid activation request
        [TestMethod]
        public async Task ActivateDiscount_WithValidData_ReturnsOk()
        {
            // Arrange
            var activeDiscountDto = new ActiveDiscountDto
            {
                Id = 1,
                RestaurantId = 1,
                IsActive = true
            };
            _mockDiscountService.Setup(service => service.ActiveAsync(activeDiscountDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ActivateDiscount(activeDiscountDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Discount activation status updated successfully.", result.Value);
        }

        // Test case for null ActiveDiscountDto
        [TestMethod]
        public async Task ActivateDiscount_WithNullData_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.ActivateDiscount(null) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, result.StatusCode);
               Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Invalid discount data.", result.Value);
        }

        // Test case for failed activation (discount not found or mismatched restaurantId)
        [TestMethod]
        public async Task ActivateDiscount_DiscountNotFound_ReturnsNotFound()
        {
            // Arrange
            var activeDiscountDto = new ActiveDiscountDto
            {
                Id = -1, // Assuming this ID does not exist
                RestaurantId = 1,
                IsActive = true
            };
            _mockDiscountService.Setup(service => service.ActiveAsync(activeDiscountDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ActivateDiscount(activeDiscountDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Discount not found or restaurant ID does not match.", result.Value);
        }

        // Test case for invalid restaurantId in ActiveDiscountDto
        [TestMethod]
        public async Task ActivateDiscount_WithInvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            var activeDiscountDto = new ActiveDiscountDto
            {
                Id = 1,
                RestaurantId = -1, // Invalid restaurantId
                IsActive = true
            };
            _mockDiscountService.Setup(service => service.ActiveAsync(activeDiscountDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ActivateDiscount(activeDiscountDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Discount not found or restaurant ID does not match.", result.Value);
        }

    }
}
