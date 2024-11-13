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
using TOPDER.Service.Utils;

namespace TOPDER.Test2.DiscountControllerTest
{
    [TestClass]
    public class GetRestaurantPagingTest
    {
        private DiscountController _controller;
        private Mock<IDiscountService> _mockDiscountService;

        [TestInitialize]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new DiscountController(_mockDiscountService.Object);
        }

        // Test case for valid inputs with discounts available
        [TestMethod]
        public async Task GetRestaurantPaging_WithValidInputs_ReturnsOkWithDiscountList()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = 1;
            
            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            
        }

       
        // Test case for invalid restaurantId (assuming -1 is invalid)
        [TestMethod]
        public async Task GetRestaurantPaging_WithInvalidRestaurantId_ReturnsOkWithEmptyList()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = -1; // Invalid restaurantId
            

            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }
    }
}
