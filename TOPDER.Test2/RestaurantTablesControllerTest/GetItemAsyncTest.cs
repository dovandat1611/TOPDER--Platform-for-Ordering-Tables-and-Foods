using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantTablesControllerTest
{
    [TestClass]
    public class GetItemAsyncTest
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
        public async Task GetItemAsync_ValidTableAndRestaurantId_ReturnsOk()
        {
            // Arrange
            var tableId = 1;
            var restaurantId = 10;
            var expectedTable = new RestaurantTableRestaurantDto
            {
                TableId = tableId,
                RoomId = 2,
                TableName = "Sample Table",
                MaxCapacity = 4,
                Description = "Test description",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.GetItemAsync(tableId, restaurantId))
                .ReturnsAsync(expectedTable);


            // Act
            var result = await _controller.GetItemAsync(tableId, restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedTable, okResult.Value);
        }

        [TestMethod]
        public async Task GetItemAsync_TableNotFound_ReturnsNotFound()
        {
            // Arrange
            var tableId = 999;
            var restaurantId = 10;

            _mockService.Setup(s => s.GetItemAsync(tableId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetItemAsync(tableId, restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy bàn với ID {tableId}.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetItemAsync_UnauthorizedAccess_ReturnsForbid()
        {
            // Arrange
            var tableId = 1;
            var restaurantId = 10;

            _mockService.Setup(s => s.GetItemAsync(tableId, restaurantId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetItemAsync(tableId, restaurantId);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }

    }
}
