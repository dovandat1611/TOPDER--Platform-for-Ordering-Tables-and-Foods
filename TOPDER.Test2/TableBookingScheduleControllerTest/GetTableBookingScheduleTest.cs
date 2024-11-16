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
    public class GetTableBookingScheduleTest
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
        public async Task GetTableBookingSchedule_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1;
            var mockSchedule = new TableBookingScheduleDto
            {
                ScheduleId = validId,
                TableId = 1,
                RestaurantId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Valid schedule"
            };

            // Mock the service to return the mock schedule for the validId
            _mockService.Setup(service => service.GetItemAsync(validId)).ReturnsAsync(mockSchedule);

            // Act
            var result = await _controller.GetTableBookingSchedule(validId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure the result is Ok
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);  // Ensure the status code is 200 (OK)

            var returnedSchedule = okResult.Value as TableBookingScheduleDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedSchedule);  // Ensure the returned value is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.ScheduleId, returnedSchedule.ScheduleId);  // Check that the ScheduleId is correct
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.TableId, returnedSchedule.TableId);  // Check the TableId
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.RestaurantId, returnedSchedule.RestaurantId);  // Check the RestaurantId
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.StartTime, returnedSchedule.StartTime);  // Check StartTime
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.EndTime, returnedSchedule.EndTime);  // Check EndTime
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedule.Notes, returnedSchedule.Notes);  // Check Notes
        }

        [TestMethod]
        public async Task GetTableBookingSchedule_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = 999;  // Assume this ID does not exist
            _mockService.Setup(service => service.GetItemAsync(invalidId)).ThrowsAsync(new KeyNotFoundException("Schedule not found"));

            // Act
            var result = await _controller.GetTableBookingSchedule(invalidId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);  // Ensure the result is NotFound
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);  // Ensure the status code is 404 (Not Found)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Schedule not found", notFoundResult.Value);  // Ensure the error message is correct
        }
    }
}
