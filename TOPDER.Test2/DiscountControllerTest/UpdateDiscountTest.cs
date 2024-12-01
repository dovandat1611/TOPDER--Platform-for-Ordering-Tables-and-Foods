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
using TOPDER.Service.Dtos.DiscountMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.DiscountControllerTest
{
    [TestClass]
    public class DiscountControllerTests
    {
        private Mock<IDiscountService> _mockDiscountService;
        private DiscountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        [TestMethod]
        public async Task UpdateDiscount_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidDto = new DiscountDto(); // Missing required fields
            _controller.ModelState.AddModelError("DiscountName", "The DiscountName field is required.");

            // Act
            var result = await _controller.UpdateDiscount(invalidDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(((SerializableError)badRequestResult.Value).ContainsKey("DiscountName"));
        }

        [TestMethod]
        public async Task UpdateDiscount_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var validDto = new DiscountDto
            {
                DiscountId = 1,
                RestaurantId = 101,
                DiscountName = "Holiday Discount",
                DiscountPercentage = 15,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                IsActive = true,
                Quantity = 100,
                Scope = "All Items",
                discountMenuDtos = new List<CreateDiscountMenuDto>
            {
                new CreateDiscountMenuDto { MenuId = 1, DiscountMenuPercentage = 10 }
            }
            };

            _mockDiscountService
                .Setup(service => service.UpdateAsync(validDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscount(validDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual("Cập Nhật Discount thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateDiscount_ReturnsNotFound_WhenDiscountDoesNotExist()
        {
            // Arrange
            var validDto = new DiscountDto
            {
                DiscountId = 999, // Non-existing ID
                RestaurantId = 101,
                DiscountName = "Non-existent Discount",
                DiscountPercentage = 15,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                IsActive = true,
                Quantity = 100,
                Scope = "All Items"
            };

            _mockDiscountService
                .Setup(service => service.UpdateAsync(validDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateDiscount(validDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.", notFoundResult.Value);
        }
    }

}
