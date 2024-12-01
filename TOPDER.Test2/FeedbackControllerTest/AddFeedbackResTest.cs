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
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.FeedbackReply;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class AddFeedbackResTest
    {
        private FeedbackController _controller;
        private Mock<IFeedbackService> _mockFeedbackService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IFeedbackReplyService> _mockFeedbackReplyService;

        // Set up mock services and controller before each test
        [TestInitialize]
        public void Setup()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockFeedbackReplyService = new Mock<IFeedbackReplyService>();

            _controller = new FeedbackController(
                _mockFeedbackService.Object,
                _mockSignalRHub.Object,
                _mockNotificationService.Object,
                _mockFeedbackReplyService.Object
            );
        }
        [TestMethod]
        public async Task AddFeedback_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidDto = new CreateFeedbackReplyDto(); // Missing required fields
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await _controller.AddFeedback(invalidDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(((SerializableError)badRequestResult.Value).ContainsKey("Content"));
        }

        [TestMethod]
        public async Task AddFeedback_ReturnsOk_WhenFeedbackReplyIsCreatedSuccessfully()
        {
            // Arrange
            var validDto = new CreateFeedbackReplyDto
            {
                FeedbackId = 1,
                RestaurantId = 101,
                Content = "Thank you for your feedback!"
            };

            var feedbackReplyResult = new { CustomerId = 1 }; // Simulate successful creation
            var notificationResult = new NotificationDto
            {
                NotificationId = 1,
                Uid = feedbackReplyResult.CustomerId,
                CreatedAt = DateTime.Now,
                Content = "New feedback reply added!",
                Type = Notification_Type.ADD_FEEDBACK,
                IsRead = false
            };

            var feedback = new FeedbackDto
            {
                FeedbackId = 1,
                OrderId = 123,
                CustomerId = 456,
                RestaurantId = 789,
                Star = 5,
                Content = "Dịch vụ rất tốt, tôi rất hài lòng!"
            };

            _mockFeedbackReplyService
                .Setup(service => service.AddAsync(validDto))
                .ReturnsAsync(feedback);

            _mockNotificationService
                .Setup(service => service.AddAsync(It.IsAny<NotificationDto>()))
                .ReturnsAsync(notificationResult);
            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _mockSignalRHub.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);
            // Act
            var result = await _controller.AddFeedback(validDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo thành công FeedbackReply", okResult.Value);
        }

        [TestMethod]
        public async Task AddFeedback_ReturnsBadRequest_WhenFeedbackReplyCreationFails()
        {
            // Arrange
            var validDto = new CreateFeedbackReplyDto
            {
                FeedbackId = 1,
                RestaurantId = 101,
                Content = "Thank you for your feedback!"
            };


            // Act
            var result = await _controller.AddFeedback(validDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create feedbackReply.", badRequestResult.Value);
        }
    }
}
