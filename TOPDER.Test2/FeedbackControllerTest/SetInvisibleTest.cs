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
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class SetInvisibleTest
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


        // Test case for feedbackId = 1 (Feedback exists and is set invisible)
        [TestMethod]
        public async Task SetInvisible_WithValidFeedbackId_ReturnsOk()
        {
            // Arrange
            var feedbackId = 1;
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 1,
                OrderId = 12345,
                CustomerId = 1001,
                RestaurantId = 2001,
                Star = 5,
                Content = "Great service and delicious food!"
            };

            _mockFeedbackService.Setup(service => service.InvisibleAsync(feedbackId))
                .ReturnsAsync(feedbackDto); // Simulate successful feedback hiding/deleting

            // Act
            var result = await _controller.SetInvisible(feedbackId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Ẩn/Xóa Feedback thành công.", result.Value); // Verify the success message
        }

        // Test case for feedbackId = -1 (Feedback does not exist)
        [TestMethod]
        public async Task SetInvisible_WithInvalidFeedbackId_ReturnsNotFound()
        {
            // Arrange
            var feedbackId = -1;
            
            // Act
            var result = await _controller.SetInvisible(feedbackId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual("Feedback with ID -1 not found.", result.Value); // Verify the error message
        }

        // Test case for feedbackId = 999 (Feedback does not exist)
        [TestMethod]
        public async Task SetInvisible_WithNonExistentFeedbackId_ReturnsNotFound()
        {
            // Arrange
            var feedbackId = 999;
            
            // Act
            var result = await _controller.SetInvisible(feedbackId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback with ID 999 not found.", result.Value); // Verify the error message
        }
    }
}
