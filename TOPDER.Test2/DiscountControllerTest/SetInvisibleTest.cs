using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.DiscountControllerTest
{
    [TestClass]
    public class SetInvisibleTest
    {
        private DiscountController _controller;
        private Mock<IDiscountService> _mockDiscountService;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        // Test case for successful invisibility setting
        [TestMethod]
        public async Task SetInvisible_WithValidIds_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = 1;
            _mockDiscountService.Setup(service => service.InvisibleAsync(discountId, restaurantId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SetInvisible(restaurantId, discountId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Ẩn/Xóa Discount thành công.", result.Value);
        }

        // Test case for discount not found or not belonging to the restaurant
        [TestMethod]
        public async Task SetInvisible_DiscountNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = 9999; // Assuming 9999 is an invalid or non-existent discountId
            _mockDiscountService.Setup(service => service.InvisibleAsync(discountId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.", result.Value);
        }

        // Test case for invalid restaurantId (e.g., negative value)
        [TestMethod]
        public async Task SetInvisible_WithInvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId
            int discountId = 1;
            _mockDiscountService.Setup(service => service.InvisibleAsync(discountId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
                Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.", result.Value);
        }

        // Test case for invalid discountId (e.g., negative value)
        [TestMethod]
        public async Task SetInvisible_WithInvalidDiscountId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int discountId = -1; // Invalid discountId
            _mockDiscountService.Setup(service => service.InvisibleAsync(discountId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, discountId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.", result.Value);
        }
    }
}
