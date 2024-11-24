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
    public class AddFeedbackTest
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

        // Test case for valid feedback
        [TestMethod]
        public async Task AddFeedback_WithValidFeedback_ReturnsOk()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = "Excellent food!"
            };

            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback created successfully.", result.Value);
        }

        // Test case for invalid model state
        [TestMethod]
        public async Task AddFeedback_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                // Simulating invalid model state by leaving the required fields empty
            };

            _controller.ModelState.AddModelError("Star", "The Star field is required."); // Simulate model state error

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // ModelState errors should be in a SerializableError
        }

        // Test case for failed feedback creation
        [TestMethod]
        public async Task AddFeedback_WithInvalidCustomerId_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = 1,
                CustomerId = 9999, // Invalid CustomerId (edge case)
                RestaurantId = 1,
                Star = 5,
                Content = "Feedback with invalid customer."
            };

            // Setup mock feedback service to simulate failure when the CustomerId is 9999
            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create feedback.", result.Value);
        }
        [TestMethod]
        public async Task AddFeedback_WithNullContent_ReturnsOk()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = 1,
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = null // Content is null
            };

            // Setup mock feedback service to simulate successful feedback creation
            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Feedback created successfully.", result.Value);
        }

        [TestMethod]
        public async Task AddFeedback_WithInvalidOrderId_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = -1, // Invalid OrderId
                CustomerId = 1,
                RestaurantId = 1,
                Star = 5,
                Content = "Great service!" // Valid content
            };

            // Setup mock feedback service to simulate failed feedback creation due to invalid OrderId
            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create feedback.", result.Value);
        }

        [TestMethod]
        public async Task AddFeedback_WithInvalidOrderIdAndMismatchedCustomerId_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = -1, // Invalid OrderId
                CustomerId = 1, // Customer ID
                RestaurantId = 2, // Mismatched RestaurantId (should not match the restaurant where the order was placed)
                Star = 5,
                Content = "Great service!" // Valid content
            };

            // Setup mock feedback service to simulate failed feedback creation due to mismatched CustomerId and OrderId
            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create feedback.", result.Value);
        }

        [TestMethod]
        public async Task AddFeedback_WithInvalidRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                OrderId = 1, // Valid OrderId
                CustomerId = 1, // Valid CustomerId
                RestaurantId = -1, // Invalid RestaurantId
                Star = 5,
                Content = "Great service!" // Valid content
            };

            // Setup mock feedback service to simulate failed feedback creation due to invalid RestaurantId
            _mockFeedbackService.Setup(service => service.AddAsync(feedbackDto)).ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.AddFeedback(feedbackDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create feedback.", result.Value);
        }
    }
}
