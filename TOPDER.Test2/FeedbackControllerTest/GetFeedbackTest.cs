using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class GetFeedbackTest
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private FeedbackController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        // Test case for orderId = -1 (Invalid Order ID)
        [TestMethod]
        public async Task GetFeedback_WithInvalidOrderId_ReturnsNotFound()
        {
            // Arrange
            var orderId = -1; // Invalid order ID
            _mockFeedbackService.Setup(service => service.GetFeedbackAsync(orderId))
                .ReturnsAsync((FeedbackDto)null); // Simulate that feedback does not exist

            // Act
            var result = await _controller.GetFeedback(orderId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Feedback for Order ID {orderId} not found.", result.Value);
        }

        // Test case for orderId = 1 (Valid Order ID)
        [TestMethod]
        public async Task GetFeedback_WithValidOrderId_ReturnsOk()
        {
            // Arrange
            var orderId = 1; // Valid order ID
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 1,
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = "Great service!"
            };
            _mockFeedbackService.Setup(service => service.GetFeedbackAsync(orderId))
                .ReturnsAsync(feedbackDto); // Simulate that feedback exists

            // Act
            var result = await _controller.GetFeedback(orderId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as FeedbackDto;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(feedbackDto.FeedbackId, response.FeedbackId);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(feedbackDto.Content, response.Content);
        }

        // Test case for orderId = 99999 (Non-existent Order ID)
        [TestMethod]
        public async Task GetFeedback_WithNonExistentOrderId_ReturnsNotFound()
        {
            // Arrange
            var orderId = 99999; // Non-existent order ID
            _mockFeedbackService.Setup(service => service.GetFeedbackAsync(orderId))
                .ReturnsAsync((FeedbackDto)null); // Simulate that feedback does not exist

            // Act
            var result = await _controller.GetFeedback(orderId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Feedback for Order ID {orderId} not found.", result.Value);
        }
    }
}