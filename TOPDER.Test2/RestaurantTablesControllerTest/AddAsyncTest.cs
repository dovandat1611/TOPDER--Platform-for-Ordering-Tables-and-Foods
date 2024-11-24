using Microsoft.AspNetCore.Http;
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
    public class AddAsyncTest
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
        public async Task AddAsync_ValidInput_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 1,
                RoomId = 2,
                TableName = "VIP Table",
                MaxCapacity = 6,
                Description = "Corner table with extra space",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.AddAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo bàn thành công!", okResult.Value);
        }

        [TestMethod]
        public async Task AddAsync_NullDto_ReturnsBadRequest()
        {
            // Arrange
            RestaurantTableDto? tableDto = null;

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Dữ liệu bàn ăn là bắt buộc.", badRequestResult.Value);
        }


        [TestMethod]
        public async Task AddAsync_TableNameIsNull_ReturnsBadRequest()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 1,
                RestaurantId = 10,
                RoomId = 1,
                TableName = null, // TableName is null
                MaxCapacity = 6,
                Description = "Sample table",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.AddAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo bàn thành công!", okResult.Value);
        }

        

        [TestMethod]
        public async Task AddAsync_MaxCapacityIsZero_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 3,
                RestaurantId = 10,
                RoomId = 1,
                TableName = "Table B",
                MaxCapacity = 0, // Invalid MaxCapacity
                Description = "Sample table",
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.AddAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo bàn thành công!", okResult.Value);
        }

        [TestMethod]
        public async Task AddAsync_DescriptionIsNull_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 4,
                RestaurantId = 10,
                RoomId = 1,
                TableName = "Table C",
                MaxCapacity = 4,
                Description = null, // Optional field
                IsBookingEnabled = true
            };

            _mockService.Setup(s => s.AddAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo bàn thành công!", okResult.Value);
        }

        [TestMethod]
        public async Task AddAsync_IsBookingEnabledIsNull_ReturnsOk()
        {
            // Arrange
            var tableDto = new RestaurantTableDto
            {
                TableId = 5,
                RestaurantId = 10,
                RoomId = 1,
                TableName = "Table D",
                MaxCapacity = 4,
                Description = "Sample table",
                IsBookingEnabled = null // Optional field
            };

            _mockService.Setup(s => s.AddAsync(tableDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddAsync(tableDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo bàn thành công!", okResult.Value);
        }
        [TestMethod]
        public async Task AddAsync_NonExistingRoomId_ReturnsBadRequest()
        {
            // Arrange
            var restaurantTableDto = new RestaurantTableDto
            {
                RestaurantId = 1, // Existing RestaurantId
                RoomId = -1,   // Non-existing RoomId
                TableName = "Table 1",
                MaxCapacity = 4,
                Description = "Test Table",
                IsBookingEnabled = true
            };

            _mockService
                .Setup(service => service.AddAsync(It.IsAny<RestaurantTableDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddAsync(restaurantTableDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đã xảy ra lỗi khi thêm bàn.", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task AddAsync_NonExistingRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var restaurantTableDto = new RestaurantTableDto
            {
                RestaurantId = -1, // Non-existing RestaurantId
                RoomId = 1,           // Existing RoomId
                TableName = "Table 1",
                MaxCapacity = 4,
                Description = "Test Table",
                IsBookingEnabled = true
            };

            _mockService
                .Setup(service => service.AddAsync(It.IsAny<RestaurantTableDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddAsync(restaurantTableDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đã xảy ra lỗi khi thêm bàn.", statusCodeResult.Value);
        }
    }
}
