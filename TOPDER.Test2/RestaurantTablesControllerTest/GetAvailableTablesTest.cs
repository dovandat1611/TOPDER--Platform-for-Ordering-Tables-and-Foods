using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;

namespace TOPDER.Test.RestaurantTablesControllerTest
{
    [TestClass]
    public class GetAvailableTablesTest
    {
        private Mock<IRestaurantTableService> _mockService;
        private RestaurantTablesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IRestaurantTableService>();
            _controller = new RestaurantTablesController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetAvailableTablesAsync_ValidTime_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            string timeReservation = "18:00";
            DateTime dateReservation = new DateTime(2024, 11, 16);

            var availableTables = new List<RestaurantTableRestaurantDto>
            {
                new RestaurantTableRestaurantDto { TableId = 1, TableName = "Table 1", MaxCapacity = 4 },
                new RestaurantTableRestaurantDto { TableId = 2, TableName = "Table 2", MaxCapacity = 2 }
            };

            // Mock the service to return a list of available tables
            _mockService.Setup(s => s.GetAvailableTablesAsync(restaurantId, It.IsAny<TimeSpan>(), dateReservation))
                        .ReturnsAsync(availableTables);

            // Act
            var result = await _controller.GetAvailableTablesAsync(restaurantId, timeReservation, dateReservation);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult
            var response = okResult.Value as List<RestaurantTableRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response); // Ensure the response is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count); // Check if we received the correct number of tables
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table 1", response[0].TableName); // Check table name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(4, response[0].MaxCapacity); // Check table capacity
        }

        [TestMethod]
        public async Task GetAvailableTablesAsync_InvalidTime_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = 1;
            string timeReservation = "invalidTime"; // Invalid time string
            DateTime dateReservation = new DateTime(2024, 11, 16);

            // Act
            var result = await _controller.GetAvailableTablesAsync(restaurantId, timeReservation, dateReservation);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult); // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thời gian đặt bàn không hợp lệ.", badRequestResult.Value); // Check error message
        }


        [TestMethod]
        public async Task GetAvailableTablesAsync_EmptyTime_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = 1;
            string timeReservation = ""; // Empty string
            DateTime dateReservation = new DateTime(2024, 11, 16);

            // Act
            var result = await _controller.GetAvailableTablesAsync(restaurantId, timeReservation, dateReservation);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult); // Ensure the result is BadRequest
        }
    }
}
