using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.NotificationControllerTest
{
    [TestClass]
    public class MarkAsReadAllTest
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IHubContext<AppHub>> _signalRHubMock;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _signalRHubMock = new Mock<IHubContext<AppHub>>();

            _controller = new NotificationController(
                _notificationServiceMock.Object,
                _signalRHubMock.Object
            );
        }

        [TestMethod]
        public async Task MarkAsReadAll_ReturnsOk_WhenNotificationsAreMarkedAsRead()
        {
            // Arrange
            int userId = 1;
            _notificationServiceMock
                .Setup(service => service.IsReadAllAsync(userId))
                .ReturnsAsync(true); // Simulate successful marking of all notifications as read

            // Act
            var result = await _controller.MarkAsReadAll(userId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thông báo đã được đánh dấu là đã đọc.", okResult.Value);
        }

        [TestMethod]
        public async Task MarkAsReadAll_ReturnsNotFound_WhenNoNotificationsFoundOrAlreadyMarkedAsRead()
        {
            // Arrange
            int userId = -1; // Simulate a user with no notifications
            _notificationServiceMock
                .Setup(service => service.IsReadAllAsync(userId))
                .ReturnsAsync(false); // Simulate no notifications or all notifications already marked as read

            // Act
            var result = await _controller.MarkAsReadAll(userId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy thông báo hoặc thông báo đã được đánh dấu là đã đọc.", notFoundResult.Value);
        }
    }
}
