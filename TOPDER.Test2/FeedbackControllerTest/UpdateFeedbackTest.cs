using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class UpdateFeedbackTest
    {
        private FeedbackController _controller;
        private Mock<IFeedbackService> _mockFeedbackService;

        // Set up mock service and controller before each test
        [TestInitialize]
        public void Setup()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        // Test case for valid feedback update
        [TestMethod]
        public async Task UpdateFeedback_WithValidFeedback_ReturnsOk()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 1,
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = "Great food, updated!"
            };

            _mockFeedbackService.Setup(service => service.UpdateAsync(feedbackDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback updated successfully.", result.Value);
        }

        // Test case for invalid ModelState
        [TestMethod]
        public async Task UpdateFeedback_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                // Simulating invalid model state (e.g., missing required field)
            };

            _controller.ModelState.AddModelError("Star", "The Star field is required.");

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // ModelState errors should be in a SerializableError
        }

        // Test case for feedback update failure (feedback not found)
        [TestMethod]
        public async Task UpdateFeedback_WithNonExistentFeedback_ReturnsNotFound()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 9999, // Simulate non-existent feedback
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 4,
                Content = "Updated but not found"
            };

            _mockFeedbackService.Setup(service => service.UpdateAsync(feedbackDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback not found.", result.Value);
        }

        // Test case for feedback update with null content
        [TestMethod]
        public async Task UpdateFeedback_WithNullContent_ReturnsOk()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                FeedbackId = 1,
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = null // Null content, but should still be updated
            };

            _mockFeedbackService.Setup(service => service.UpdateAsync(feedbackDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateFeedback(feedbackDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback updated successfully.", result.Value);
        }
    }
}
