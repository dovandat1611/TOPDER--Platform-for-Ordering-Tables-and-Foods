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
    public class UpdateAsyncTest
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
        public async Task UpdateAsync_ValidTable_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = 2,
                TableName = "Updated Table",
                MaxCapacity = 6,
                Description = "Updated description",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update table thành công", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateAsync_NullTableDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateAsync(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Dữ liệu bàn ăn là bắt buộc.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateAsync_TableNotFound_ReturnsNotFound()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = -1,
                RestaurantId = 10,
                RoomId = 1,
                TableName = "Non-existent Table",
                MaxCapacity = 4,
                Description = null,
                IsBookingEnabled = false
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(notFoundResult);
               Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy bàn với ID {tableDto.TableId} hoặc không thuộc nhà hàng đã chỉ định.", notFoundResult.Value);
        }
        [TestMethod]
        public async Task UpdateAsync_NullRoomId_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = null,
                TableName = "Test Table",
                MaxCapacity = 4,
                Description = "Test Description",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update table thành công", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateAsync_NullTableName_ReturnsBadRequest()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = 2,
                TableName = null,
                MaxCapacity = 4,
                Description = "Test Description",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update table thành công", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateAsync_NullDescription_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = 2,
                TableName = "Test Table",
                MaxCapacity = 4,
                Description = null,
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update table thành công", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateAsync_NullIsBookingEnabled_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = 2,
                TableName = "Test Table",
                MaxCapacity = 4,
                Description = "Test Description",
                IsBookingEnabled = null
            };

            _mockService.Setup(s => s.UpdateAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update table thành công", okResult.Value);
        }

    }
}
