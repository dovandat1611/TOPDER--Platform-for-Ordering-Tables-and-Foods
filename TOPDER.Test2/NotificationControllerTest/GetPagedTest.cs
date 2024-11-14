using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.NotificationControllerTest
{
    [TestClass]
    public class GetPagedTest
    {
        private Mock<INotificationService> _notificationService;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationService = new Mock<INotificationService>();
            _controller = new NotificationController(_notificationService.Object);
        }

        // Test 1: Valid parameters - should return a list of notifications
        [TestMethod]
        public async Task GetPaged_ValidParameters_ReturnsOkResultWithNotifications()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int userId = 1;

            var notifications = new List<NotificationDto>
            {
                new NotificationDto { NotificationId = 1, Uid = userId, Content = "Test Notification 1", Type = Notification_Type.SYSTEM_ADD },
                new NotificationDto { NotificationId = 2, Uid = userId, Content = "Test Notification 2", Type = Notification_Type.SYSTEM_SUB }
            };

            var paginatedNotifications = new PaginatedList<NotificationDto>(notifications, notifications.Count, pageNumber, pageSize);

            _notificationService.Setup(service => service.GetPagingAsync(pageNumber, pageSize, userId))
                .ReturnsAsync(paginatedNotifications);
            // Act
            var result = await _controller.GetPaged(pageNumber, pageSize, userId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(List<NotificationDto>));
            var notificationList = okResult.Value as List<NotificationDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(notifications.Count, notificationList.Count);
        }

        // Test 2: No notifications for the user - should return an empty list
        [TestMethod]
        public async Task GetPaged_NoNotifications_ReturnsOkResultWithEmptyList()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int userId = 9999;

            var notifications = new List<NotificationDto>();  // No notifications for the user
            var paginatedNotifications = new PaginatedList<NotificationDto>(notifications, notifications.Count, pageNumber, pageSize);

            _notificationService.Setup(service => service.GetPagingAsync(pageNumber, pageSize, userId))
                .ReturnsAsync(paginatedNotifications);

            // Act
            var result = await _controller.GetPaged(pageNumber, pageSize, userId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(List<NotificationDto>));
            var notificationList = okResult.Value as List<NotificationDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, notificationList.Count);  // Ensure the list is empty
        }
    }
}
