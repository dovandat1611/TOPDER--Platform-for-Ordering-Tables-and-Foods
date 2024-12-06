using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.RestaurantTablesControllerTest
{
    [TestClass]
    public class SearchPagingAsyncTest
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
        public async Task SearchPagingAsync_Success_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? tableName = "Table1";

            var paginatedList = new PaginatedList<RestaurantTableRestaurantDto>(
                new List<RestaurantTableRestaurantDto>
                {
                    new RestaurantTableRestaurantDto { TableId = 1, TableName = "Table1", MaxCapacity = 4 }
                },
                1, // count of items (for example, 1 item)
                1, // page index
                10 // page size
            );

            _mockService.Setup(s => s.GetTableListAsync(pageNumber, pageSize, restaurantId, tableName))
                        .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPagingAsync(restaurantId, pageNumber, pageSize, tableName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<RestaurantTableRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table1", response.Items[0].TableName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
        }

        [TestMethod]
        public async Task SearchPagingAsync_NoTables_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? tableName = null; // no table name filter

            var paginatedList = new PaginatedList<RestaurantTableRestaurantDto>(
                new List<RestaurantTableRestaurantDto>(),
                0, // count of items (no items in this case)
                1, // page index
                10 // page size
            );

            _mockService.Setup(s => s.GetTableListAsync(pageNumber, pageSize, restaurantId, tableName))
                        .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPagingAsync(restaurantId, pageNumber, pageSize, tableName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<RestaurantTableRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.TotalPages);
        }
    }
}
