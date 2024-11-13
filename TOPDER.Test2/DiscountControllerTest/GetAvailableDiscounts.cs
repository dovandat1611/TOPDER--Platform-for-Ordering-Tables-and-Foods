using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Services;

namespace TOPDER.Test2.DiscountControllerTest
{
    [TestClass]
    public class GetAvailableDiscountsTests
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
        public async Task GetAvailableDiscounts_WithValidParameters_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int customerId = 1;
            decimal totalPrice = 100m;

            var discounts = new List<AvailableDiscountDto>
    {
        new AvailableDiscountDto
        {
            DiscountId = 1,
            RestaurantId = restaurantId,
            DiscountPercentage = 10,
            DiscountName = "Sample Discount",
            ApplicableTo = DiscountApplicableTo.ALL_CUSTOMERS,
            Scope = DiscountScope.ENTIRE_ORDER,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            Quantity = 50
        }
    };

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(discounts);

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(discounts, result.Value);
        }

        // Test case for invalid restaurantId (restaurantId = -1)
        [TestMethod]
        public async Task GetAvailableDiscounts_WithInvalidRestaurantId_ReturnsOkWithEmptyList()
        {
            // Arrange
            int restaurantId = -1;
            int customerId = 1;
            decimal totalPrice = 100m;

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(new List<AvailableDiscountDto>());

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, ((List<AvailableDiscountDto>)result.Value).Count);
        }

        // Test case for customer without applicable discounts (e.g., totalPrice too high)
        [TestMethod]
        public async Task GetAvailableDiscounts_NoApplicableDiscounts_ReturnsOkWithEmptyList()
        {
            // Arrange
            int restaurantId = 1;
            int customerId = 1;
            decimal totalPrice = 5000m; // Price that doesn't meet any discount criteria

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(new List<AvailableDiscountDto>());

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, ((List<AvailableDiscountDto>)result.Value).Count);
        }

        // Test case for totalPrice being negative (e.g., -100)
        [TestMethod]
        public async Task GetAvailableDiscounts_WithNegativeTotalPrice_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int customerId = 1;
            decimal totalPrice = -100m;

            var discounts = new List<AvailableDiscountDto>
    {
        new AvailableDiscountDto
        {
            DiscountId = 1,
            RestaurantId = restaurantId,
            DiscountPercentage = 10,
            DiscountName = "Sample Discount",
            ApplicableTo = DiscountApplicableTo.ALL_CUSTOMERS,
            Scope = DiscountScope.ENTIRE_ORDER,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            Quantity = 50
        }
    };

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(discounts);

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(discounts, result.Value);
        }

        // Test case for totalPrice being exactly zero
        [TestMethod]
        public async Task GetAvailableDiscounts_WithZeroTotalPrice_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int customerId = 1;
            decimal totalPrice = 0m;

            var discounts = new List<AvailableDiscountDto>
    {
        new AvailableDiscountDto
        {
            DiscountId = 1,
            RestaurantId = restaurantId,
            DiscountPercentage = 10,
            DiscountName = "Sample Discount",
            ApplicableTo = DiscountApplicableTo.ALL_CUSTOMERS,
            Scope = DiscountScope.ENTIRE_ORDER,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            Quantity = 50
        }
    };

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(discounts);

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(discounts, result.Value);
        }


        [TestMethod]
        public async Task GetAvailableDiscounts_NonCustomer_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int customerId = -1;
            decimal totalPrice = 0m;

            var discounts = new List<AvailableDiscountDto>
    {
        new AvailableDiscountDto
        {
            DiscountId = 1,
            RestaurantId = restaurantId,
            DiscountPercentage = 10,
            DiscountName = "Sample Discount",
            ApplicableTo = DiscountApplicableTo.ALL_CUSTOMERS,
            Scope = DiscountScope.ENTIRE_ORDER,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            Quantity = 50
        }
    };

            _mockDiscountService.Setup(service => service.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice))
                .ReturnsAsync(discounts);

            // Act
            var result = await _controller.GetAvailableDiscounts(restaurantId, customerId, totalPrice) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(discounts, result.Value);
        }
    }

}
