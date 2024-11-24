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
    public class DeleteTest
    {
        private Mock<INotificationService> _notificationService;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationService = new Mock<INotificationService>();
            _controller = new NotificationController(_notificationService.Object);
        }

        // Test 1: Successful Deletion
        [TestMethod]
        public async Task Delete_ValidNotification_ReturnsOk()
        {
            // Arrange
            var userId = 1;
            var notificationId = 1;

            // Mock RemoveAsync to return true (successful deletion)
            _notificationService.Setup(service => service.RemoveAsync(notificationId, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(userId, notificationId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Xóa thông báo thành công.", okResult.Value);
        }

        // Test 2: Notification Not Found (or user doesn't own the notification)
        [TestMethod]
        public async Task Delete_NotificationNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var notificationId = -1;

            // Mock RemoveAsync to return false (notification not found or doesn't belong to the user)
            _notificationService.Setup(service => service.RemoveAsync(notificationId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(userId, notificationId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy thông báo hoặc thông báo không thuộc về user.", notFoundResult.Value);
        }

        // Test 3: Invalid UserId (doesn't exist or unauthorized)
        [TestMethod]
        public async Task Delete_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            var userId = -1; // Invalid user ID
            var notificationId = 1;

            // Mock RemoveAsync to return false (notification not found or user unauthorized)
            _notificationService.Setup(service => service.RemoveAsync(notificationId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(userId, notificationId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy thông báo hoặc thông báo không thuộc về user.", notFoundResult.Value);
        }
    }
}
