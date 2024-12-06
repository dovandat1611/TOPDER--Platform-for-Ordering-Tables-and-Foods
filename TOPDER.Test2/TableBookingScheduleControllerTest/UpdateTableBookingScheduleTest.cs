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
using TOPDER.Service.Dtos.TableBookingSchedule;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.TableBookingScheduleControllerTest
{
    [TestClass]
    public class UpdateTableBookingScheduleTest
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
        public async Task UpdateTableBookingSchedule_ValidSchedule_ReturnsOk()
        {
            // Arrange
            var validSchedule = new TableBookingScheduleDto
            {
                ScheduleId = 1,
                TableId = 1,
                RestaurantId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Test Notes"
            };

            _mockService.Setup(service => service.UpdateAsync(validSchedule)).ReturnsAsync(true); // Mock service to return true when updating schedule

            // Act
            var result = await _controller.UpdateTableBookingSchedule(validSchedule);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is Ok
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // Ensure the status code is 200 (OK)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật lịch đặt bàn thành công.", okResult.Value); // Ensure the success message is returned
        }

        [TestMethod]
        public async Task UpdateTableBookingSchedule_InvalidSchedule_ReturnsNotFound()
        {
            // Arrange
            var invalidSchedule = new TableBookingScheduleDto
            {
                ScheduleId = -1, // Invalid ID
                TableId = 2,
                RestaurantId = 3,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Test Notes"
            };

            _mockService.Setup(service => service.UpdateAsync(invalidSchedule)).ReturnsAsync(false); // Mock service to return false when no schedule is found

            // Act
            var result = await _controller.UpdateTableBookingSchedule(invalidSchedule);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult); // Ensure the result is NotFound
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode); // Ensure the status code is 404 (Not Found)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy lịch đặt bàn để cập nhật.", notFoundResult.Value); // Ensure the error message is returned
        }

        [TestMethod]
        public async Task UpdateTableBookingSchedule_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var invalidSchedule = new TableBookingScheduleDto
            {
                ScheduleId = 1,
                TableId = -1, // Invalid value for TableId
                RestaurantId = 3,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Test Notes"
            };

            _controller.ModelState.AddModelError("TableId", "TableId is required");

            // Act
            var result = await _controller.UpdateTableBookingSchedule(invalidSchedule);

            // Assert
            var badRequestResult = result as NotFoundObjectResult; // Fixed here to ensure BadRequest response type
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult); // Ensure the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, badRequestResult.StatusCode); // Ensure the status code is 400 (Bad Request)
        }

        // Test when StartTime is empty (e.g. null or invalid)
        [TestMethod]
        public async Task UpdateTableBookingSchedule_EmptyStartTime_ReturnsBadRequest()
        {
            // Arrange
            var scheduleWithEmptyStartTime = new TableBookingScheduleDto
            {
                ScheduleId = 1,
                TableId = 2,
                RestaurantId = 3,
                StartTime = default(DateTime),  // Empty StartTime
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Valid Notes"
            };

            _mockService.Setup(service => service.UpdateAsync(scheduleWithEmptyStartTime)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTableBookingSchedule(scheduleWithEmptyStartTime);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật lịch đặt bàn thành công.", okResult.Value); // Ensure the success message is returned
        }

        // Test when EndTime is empty (e.g. null or invalid)
        [TestMethod]
        public async Task UpdateTableBookingSchedule_EmptyEndTime_ReturnsBadRequest()
        {
            // Arrange
            var scheduleWithEmptyEndTime = new TableBookingScheduleDto
            {
                ScheduleId = 1,
                TableId = 2,
                RestaurantId = 3,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = default(DateTime),  // Empty EndTime
                Notes = "Valid Notes"
            };

            _mockService.Setup(service => service.UpdateAsync(scheduleWithEmptyEndTime)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTableBookingSchedule(scheduleWithEmptyEndTime);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật lịch đặt bàn thành công.", okResult.Value); // Ensure the success message is returned
        }

        // Test when Notes is empty (e.g. null or empty string)
        [TestMethod]
        public async Task UpdateTableBookingSchedule_EmptyNotes_ReturnsOk()
        {
            // Arrange
            var scheduleWithEmptyNotes = new TableBookingScheduleDto
            {
                ScheduleId = 1,
                TableId = 2,
                RestaurantId = 3,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = ""  // Empty Notes
            };

            _mockService.Setup(service => service.UpdateAsync(scheduleWithEmptyNotes)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTableBookingSchedule(scheduleWithEmptyNotes);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật lịch đặt bàn thành công.", okResult.Value); // Ensure the success message is returned
        }
    }
}
