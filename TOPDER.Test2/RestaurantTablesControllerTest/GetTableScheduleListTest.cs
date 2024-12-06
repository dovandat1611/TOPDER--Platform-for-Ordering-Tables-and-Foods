using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantTablesControllerTest
{
    [TestClass]
    public class GetTableScheduleListTest
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
        public async Task GetTableScheduleList_Success_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            var expectedTables = new List<RestaurantTableRestaurantDto>
            {
                new RestaurantTableRestaurantDto { TableId = 1, TableName = "Table 1", MaxCapacity = 4 },
                new RestaurantTableRestaurantDto { TableId = 2, TableName = "Table 2", MaxCapacity = 2 }
            };

            // Setup mock to return expected tables
            _mockService.Setup(s => s.GetTableScheduleAsync(restaurantId))
                        .ReturnsAsync(expectedTables);

            // Act
            var result = await _controller.GetTableScheduleList(restaurantId);

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
        public async Task GetTableScheduleList_NoTables_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = -1;
            var expectedTables = new List<RestaurantTableRestaurantDto>(); // Empty list, no tables

            // Setup mock to return empty list
            _mockService.Setup(s => s.GetTableScheduleAsync(restaurantId))
                        .ReturnsAsync(expectedTables);

            // Act
            var result = await _controller.GetTableScheduleList(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult
            var response = okResult.Value as List<RestaurantTableRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response); // Ensure the response is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Count); // Ensure the list is empty
        }
    }
}
