using AutoMapper;
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
    public class GetTableBookingSchedulesTest
    {
        private Mock<ITableBookingScheduleService> _mockService;
        private Mock<IMapper> _mockMapper;
        private TableBookingScheduleController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockService = new Mock<ITableBookingScheduleService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new TableBookingScheduleController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetTableBookingSchedules_ValidRestaurantId_ReturnsOk()
        {
            // Arrange
            int validRestaurantId = 1;
            var mockSchedules = new List<TableBookingScheduleViewDto>
        {
            new TableBookingScheduleViewDto
            {
                ScheduleId = 1,
                TableId = 1,
                RestaurantId = validRestaurantId,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Notes = "Valid schedule"
            },
            new TableBookingScheduleViewDto
            {
                ScheduleId = 2,
                TableId = 2,
                RestaurantId = validRestaurantId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                Notes = "Another valid schedule"
            }
        };

            // Mock the service to return the mock schedules for the valid restaurantId
            _mockService.Setup(service => service.GetTableBookingScheduleListAsync(validRestaurantId)).ReturnsAsync(mockSchedules);

            // Act
            var result = await _controller.GetTableBookingSchedules(validRestaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure the result is Ok
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);  // Ensure the status code is 200 (OK)

            var returnedSchedules = okResult.Value as List<TableBookingScheduleViewDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedSchedules);  // Ensure the returned value is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules.Count, returnedSchedules.Count);  // Ensure the count is correct

            // Optionally, verify individual schedules
            for (int i = 0; i < mockSchedules.Count; i++)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].ScheduleId, returnedSchedules[i].ScheduleId);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].TableId, returnedSchedules[i].TableId);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].RestaurantId, returnedSchedules[i].RestaurantId);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].StartTime, returnedSchedules[i].StartTime);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].EndTime, returnedSchedules[i].EndTime);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockSchedules[i].Notes, returnedSchedules[i].Notes);
            }
        }

        [TestMethod]
        public async Task GetTableBookingSchedules_InvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int invalidRestaurantId = -1;  // Assume this ID does not exist
            _mockService.Setup(service => service.GetTableBookingScheduleListAsync(invalidRestaurantId)).ReturnsAsync(new List<TableBookingScheduleViewDto>());

            // Act
            var result = await _controller.GetTableBookingSchedules(invalidRestaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure the result is Ok
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode);  // Ensure the status code is 200 (OK)

            var returnedSchedules = okResult.Value as List<TableBookingScheduleViewDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedSchedules);  // Ensure the returned value is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, returnedSchedules.Count);  // Ensure the list is empty
        }
    }
}
