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

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class SetInvisibleTest
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private FeedbackController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        // Test case for feedbackId = 1 (Feedback exists and is set invisible)
        [TestMethod]
        public async Task SetInvisible_WithValidFeedbackId_ReturnsOk()
        {
            // Arrange
            var feedbackId = 1;
            _mockFeedbackService.Setup(service => service.InvisibleAsync(feedbackId))
                .ReturnsAsync(true); // Simulate successful feedback hiding/deleting

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
            _mockFeedbackService.Setup(service => service.InvisibleAsync(feedbackId))
                .ReturnsAsync(false); // Simulate feedback not found

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
            _mockFeedbackService.Setup(service => service.InvisibleAsync(feedbackId))
                .ReturnsAsync(false); // Simulate feedback not found

            // Act
            var result = await _controller.SetInvisible(feedbackId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback with ID 999 not found.", result.Value); // Verify the error message
        }
    }
}
