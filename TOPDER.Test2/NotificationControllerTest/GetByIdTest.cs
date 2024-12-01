using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Dtos.Notification;
using Moq;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using Microsoft.AspNetCore.SignalR;
using TOPDER.Service.Hubs;

namespace TOPDER.Test2.NotificationControllerTest
{
    [TestClass]
    public class GetByIdTest
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
        public async Task GetById_NotificationExists_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            int notificationId = 1;
            var notification = new NotificationDto
            {
                NotificationId = notificationId,
                Uid = userId,
                Content = "Thông báo thành công",
                Type = Notification_Type.SYSTEM_ADD
            };

            // Mock _notificationService.GetItemAsync
            _notificationServiceMock.Setup(service => service.GetItemAsync(notificationId, userId))
                .ReturnsAsync(notification);

            // Act
            var result = await _controller.GetById(userId, notificationId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(notification, okResult.Value);
        }

        [TestMethod]
        public async Task GetById_NotificationNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            int notificationId = -1;

            // Mock _notificationService.GetItemAsync
            _notificationServiceMock.Setup(service => service.GetItemAsync(notificationId, userId))
                .ThrowsAsync(new KeyNotFoundException("Notification not found"));

            // Act
            var result = await _controller.GetById(userId, notificationId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Notification not found", notFoundResult.Value);
        }



        [TestMethod]
        public async Task GetById_UnauthorizedAccess_ReturnsForbid()
        {
            // Arrange
            int userId = -1;
            int notificationId = 1;

            // Mock _notificationService.GetItemAsync
            _notificationServiceMock.Setup(service => service.GetItemAsync(notificationId, userId))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.GetById(userId, notificationId);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }
    }
}
