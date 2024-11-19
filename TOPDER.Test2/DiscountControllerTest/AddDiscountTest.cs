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
    public class AddDiscountTest
    {
        private Mock<IDiscountService> _mockDiscountService;
        private DiscountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        // Test case for successful discount creation
        [TestMethod]
        public async Task AddDiscount_Success_ReturnsOkResult()
        {
            // Arrange
            var discountDto = new DiscountDto
            {
                DiscountId = 0,
                RestaurantId = 1,
                DiscountPercentage = 10,
                DiscountName = "Summer Sale",
                ApplicableTo = "Food",
                ApplyType = "Percentage",
                MinOrderValue = 100,
                MaxOrderValue = 500,
                Scope = "All",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Description = "Discount for summer",
                IsActive = true,
                Quantity = 100,
                discountMenuDtos = new List<CreateDiscountMenuDto>
                {
                    new CreateDiscountMenuDto { /* Set properties */ }
                }
            };

            _mockDiscountService.Setup(service => service.AddAsync(discountDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddDiscount(discountDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
        }

        // Test case for invalid model state
        [TestMethod]
        public async Task AddDiscount_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var discountDto = new DiscountDto
            {
                DiscountId = 0,
                RestaurantId = 1,
                DiscountPercentage = null, // Invalid because DiscountPercentage is required
                DiscountName = "Summer Sale",
                ApplicableTo = "Food",
                ApplyType = "Percentage",
                MinOrderValue = 100,
                MaxOrderValue = 500,
                Scope = "All",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Description = "Discount for summer",
                IsActive = true,
                Quantity = 100,
                discountMenuDtos = new List<CreateDiscountMenuDto>
                {
                    new CreateDiscountMenuDto { /* Set properties */ }
                }
            };

            _controller.ModelState.AddModelError("DiscountPercentage", "Discount percentage is required");

            // Act
            var result = await _controller.AddDiscount(discountDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult.Value);
        }

        // Test case for ArgumentException (e.g., invalid input data)
        [TestMethod]
        public async Task AddDiscount_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var discountDto = new DiscountDto
            {
                DiscountId = 0,
                RestaurantId = 1,
                DiscountPercentage = 10,
                DiscountName = "Summer Sale",
                ApplicableTo = "Food",
                ApplyType = "Percentage",
                MinOrderValue = 100,
                MaxOrderValue = 500,
                Scope = "All",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Description = "Discount for summer",
                IsActive = true,
                Quantity = 100,
                discountMenuDtos = new List<CreateDiscountMenuDto>
                {
                    new CreateDiscountMenuDto { /* Set properties */ }
                }
            };

            _mockDiscountService.Setup(service => service.AddAsync(discountDto)).ThrowsAsync(new ArgumentException("Invalid data"));

            // Act
            var result = await _controller.AddDiscount(discountDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
        }

        [TestMethod]
        public async Task AddDiscount_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var discountDto = new DiscountDto
            {
                DiscountId = 1,
                RestaurantId = 1,
                DiscountPercentage = 10,
                DiscountName = "Summer Sale",
                ApplicableTo = "Food",
                ApplyType = "Percentage",
                MinOrderValue = 100,
                MaxOrderValue = 500,
                Scope = "All",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Description = "Discount for summer",
                IsActive = true,
                Quantity = 100,
                discountMenuDtos = new List<CreateDiscountMenuDto>
        {
            new CreateDiscountMenuDto { /* Set properties */ }
        }
            };

            _mockDiscountService.Setup(service => service.AddAsync(discountDto)).ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act
            var result = await _controller.AddDiscount(discountDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult)); // Check for ObjectResult, not StatusCodeResult
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task AddDiscount_StartDateLaterThanEndDate_ReturnsBadRequestWithErrorMessage()
        {
            // Arrange
            var discountDto = new DiscountDto
            {
                DiscountId = 1,
                RestaurantId = 1,
                DiscountPercentage = 10,
                DiscountName = "Summer Sale",
                ApplicableTo = "Food",
                ApplyType = "Percentage",
                MinOrderValue = 100,
                MaxOrderValue = 500,
                Scope = "All",
                StartDate = DateTime.Now.AddMonths(1), // StartDate is later
                EndDate = DateTime.Now, // EndDate is earlier
                Description = "Discount for summer",
                IsActive = true,
                Quantity = 100,
                discountMenuDtos = new List<CreateDiscountMenuDto> { new CreateDiscountMenuDto() }
            };

            // Act
            var result = await _controller.AddDiscount(discountDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult)); // Expected BadRequest when validation fails
            var badRequestResult = result as BadRequestObjectResult;

            // Ensure the error message is in the expected format
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult.Value);

        }
    }
}
