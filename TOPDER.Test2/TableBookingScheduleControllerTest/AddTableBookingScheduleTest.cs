using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.TableBookingSchedule;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.TableBookingScheduleControllerTest
{
    [TestClass]
    public class AddTableBookingScheduleTest
    {
        private Mock<ITableBookingScheduleService> _mockService;
        private TableBookingScheduleController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockService = new Mock<ITableBookingScheduleService>();
            _controller = new TableBookingScheduleController(_mockService.Object);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_NoTableIds_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int>(), // No TableIds provided
                RestaurantId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Notes = "Test note"
            };

            // Act
            var result = await _controller.AddTableBookingSchedule(createDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("TableId không được null.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_ScheduleAdded_ReturnsOk()
        {
            // Arrange
            var createDto = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int> { 1, 2 },
                RestaurantId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Notes = "Test note"
            };

            _mockService.Setup(s => s.AddAsync(createDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddTableBookingSchedule(createDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm lịch đặt bàn thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_EmptyRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int> { 1 }, // Valid TableIds
                RestaurantId = -1, // Empty RestaurantId
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Valid notes"
            };

            // Act
            var result = await _controller.AddTableBookingSchedule(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm lịch đặt bàn thất bại.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_EmptyStartTime_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int> { 1 }, // Valid TableIds
                RestaurantId = 1,
                StartTime = default(DateTime), // Empty StartTime (default)
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Valid notes"
            };

            // Act
            var result = await _controller.AddTableBookingSchedule(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm lịch đặt bàn thất bại.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_EmptyEndTime_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int> { 1 }, // Valid TableIds
                RestaurantId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = default(DateTime), // Empty EndTime (default)
                Notes = "Valid notes"
            };

            // Act
            var result = await _controller.AddTableBookingSchedule(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm lịch đặt bàn thất bại.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTableBookingSchedule_EmptyNotes_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateTableBookingScheduleDto
            {
                TableIds = new List<int> { 1 }, // Valid TableIds
                RestaurantId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = string.Empty // Empty Notes
            };

            // Act
            var result = await _controller.AddTableBookingSchedule(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm lịch đặt bàn thất bại.", badRequestResult.Value);
        }
    }
}
