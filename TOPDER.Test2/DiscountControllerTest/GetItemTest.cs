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
    public class GetItemTest
    {
        private DiscountController _controller;
        private Mock<IDiscountService> _mockDiscountService;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        // Test case for valid restaurantId and discountId
        [TestMethod]
        public async Task GetItem_WithValidIds_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = 1;

            // Act
            var result = await _controller.GetItem(restaurantId, discountId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
             Microsoft.VisualStudio.TestTools.UnitTesting.  Assert.AreEqual(200, result.StatusCode);
        }

        // Test case for non-existent discount (NotFound)
        [TestMethod]
        public async Task GetItem_WithInvalidDiscountId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = -1;

            _mockDiscountService.Setup(service => service.GetItemAsync(discountId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException("Discount not found"));

            // Act
            var result = await _controller.GetItem(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Discount not found", result.Value);
        }

        // Test case for unauthorized access (Forbid)
        [TestMethod]
        public async Task GetItem_WithUnauthorizedAccess_ReturnsForbid()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = 1;

            _mockDiscountService.Setup(service => service.GetItemAsync(discountId, restaurantId))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.GetItem(restaurantId, discountId) as ForbidResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
        }

        // Test case for invalid restaurantId (assuming -1 is invalid)
        [TestMethod]
        public async Task GetItem_WithInvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = -1;
            int discountId = 1;

            _mockDiscountService.Setup(service => service.GetItemAsync(discountId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException("Restaurant not found"));

            // Act
            var result = await _controller.GetItem(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant not found", result.Value);
        }

        // Test case for valid discountId but non-matching restaurantId
        [TestMethod]
        public async Task GetItem_WithValidDiscountIdAndNonMatchingRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 2; // Non-matching restaurant ID
            int discountId = 1;

            _mockDiscountService.Setup(service => service.GetItemAsync(discountId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException("Discount not associated with the restaurant"));

            // Act
            var result = await _controller.GetItem(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Discount not associated with the restaurant", result.Value);
        }
    }
}
