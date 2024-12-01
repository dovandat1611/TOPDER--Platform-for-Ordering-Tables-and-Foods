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

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class SetInvisibleFeedbackReplyTest
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
        public async Task SetInvisibleFeedbackReply_ReturnsOk_WhenReplyIsSuccessfullyHidden()
        {
            // Arrange
            int replyId = 1;
            _mockFeedbackReplyService
                .Setup(service => service.InvisibleAsync(replyId))
                .ReturnsAsync(true); // Simulate successful operation

            // Act
            var result = await _controller.SetInvisibleFeedbackReply(replyId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Ẩn/Xóa FeedbackReply thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task SetInvisibleFeedbackReply_ReturnsNotFound_WhenReplyDoesNotExist()
        {
            // Arrange
            int replyId = -1; // Simulate non-existent reply ID
            _mockFeedbackReplyService
                .Setup(service => service.InvisibleAsync(replyId))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.SetInvisibleFeedbackReply(replyId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Feedback with ID {replyId} not found.", notFoundResult.Value);
        }
    }
}
