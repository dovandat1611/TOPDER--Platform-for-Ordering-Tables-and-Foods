using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.TableBookingScheduleControllerTest
{
    [TestClass]
    public class RemoveTableBookingScheduleTest
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
        public async Task RemoveTableBookingSchedule_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1;
            _mockService.Setup(service => service.RemoveAsync(validId)).ReturnsAsync(true); // Mock service to return true when removing the schedule

            // Act
            var result = await _controller.RemoveTableBookingSchedule(validId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is Ok
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // Ensure the status code is 200 (OK)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Xóa lịch đặt bàn thành công.", okResult.Value); // Ensure the success message is returned
        }

        [TestMethod]
        public async Task RemoveTableBookingSchedule_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1; // Invalid ID that does not exist
            _mockService.Setup(service => service.RemoveAsync(invalidId)).ReturnsAsync(false); // Mock service to return false when no schedule is removed

            // Act
            var result = await _controller.RemoveTableBookingSchedule(invalidId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult); // Ensure the result is NotFound
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode); // Ensure the status code is 404 (Not Found)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy lịch đặt bàn.", notFoundResult.Value); // Ensure the error message is returned
        }
    }
}
