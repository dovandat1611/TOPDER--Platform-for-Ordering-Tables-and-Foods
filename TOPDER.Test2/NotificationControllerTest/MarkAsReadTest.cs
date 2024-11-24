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
using TOPDER.Service.IServices;

namespace TOPDER.Test2.NotificationControllerTest
{
    [TestClass]
    public class MarkAsReadTest
    {
        private Mock<INotificationService> _notificationService;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationService = new Mock<INotificationService>();
            _controller = new NotificationController(_notificationService.Object);
        }

        // Test 1: Notification marked as read successfully
        [TestMethod]
        public async Task MarkAsRead_ValidNotification_ReturnsOkResult()
        {
            // Arrange
            int notificationId = 1;
            _notificationService.Setup(service => service.IsReadAsync(notificationId))
                .ReturnsAsync(true); // Simulate that the notification was marked as read

            // Act
            var result = await _controller.MarkAsRead(notificationId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Thông báo đã được đánh dấu là đã đọc.", okResult.Value);
        }

        // Test 2: Notification not found or already marked as read
        [TestMethod]
        public async Task MarkAsRead_NotificationNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int notificationId = -1;
            _notificationService.Setup(service => service.IsReadAsync(notificationId))
                .ReturnsAsync(false); // Simulate that the notification was not found or already marked as read

            // Act
            var result = await _controller.MarkAsRead(notificationId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy thông báo hoặc thông báo đã được đánh dấu là đã đọc.", notFoundResult.Value);
        }
    }
}
