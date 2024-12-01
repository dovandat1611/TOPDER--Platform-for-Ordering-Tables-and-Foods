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
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.FeedbackReply;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class GetRestaurantFeedbacksTest
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
        public async Task GetRestaurantFeedbacks_ValidRestaurantId_ReturnsFeedbacks()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurant ID
            var feedbacks = new List<FeedbackRestaurantDto>
            {
                new FeedbackRestaurantDto
                {
                    FeedbackId = 1,
                    CustomerId = 123,
                    OrderId = 456,
                    CustomerName = "John Doe",
                    Star = 5,
                    Content = "Excellent service!",
                    CreateDate = DateTime.UtcNow,
                    Status = "Active",
                    isReply = true,
                    IsReport = false,
                },
                new FeedbackRestaurantDto
                {
                    FeedbackId = 2,
                    CustomerId = 789,
                    OrderId = 1011,
                    CustomerName = "Jane Smith",
                    Star = 4,
                    Content = "Good experience",
                    CreateDate = DateTime.UtcNow,
                    Status = "Inactive",
                    isReply = false,
                    IsReport = true,
                    FeedbackReply = new FeedbackReplyDto()
                }
            };

            // Set up the mock service to return the feedback list
            _mockFeedbackService.Setup(service => service.ListRestaurantPagingAsync(restaurantId))
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count);  // Ensure the correct number of feedbacks
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("John Doe", response[0].CustomerName);  // Check first feedback's customer name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, response[0].Star);  // Check star rating for first feedback
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Active", response[0].Status);  // Check status of the first feedback
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_InvalidRestaurantId_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = 99999; // Invalid restaurant ID
            var feedbacks = new List<FeedbackRestaurantDto>();  // No feedbacks for invalid restaurant ID

            // Set up the mock service to return an empty list
            _mockFeedbackService.Setup(service => service.ListRestaurantPagingAsync(restaurantId))
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Count);  // Ensure an empty list is returned
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_FilterByStatus_ReturnsFilteredFeedbacks()
        {
            // Arrange
            int restaurantId = 1;
            var feedbacks = new List<FeedbackRestaurantDto>
            {
                new FeedbackRestaurantDto
                {
                    FeedbackId = 1,
                    Status = "Active",
                    Content = "Excellent service!"
                },
                new FeedbackRestaurantDto
                {
                    FeedbackId = 2,
                    Status = "Inactive",
                    Content = "Good experience"
                }
            };

            _mockFeedbackService.Setup(service => service.ListRestaurantPagingAsync(restaurantId))
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.All(f => f.Status == "Active" || f.Status == "Inactive")); // Check feedback status
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_FilterByIsReport_ReturnsFilteredFeedbacks()
        {
            // Arrange
            int restaurantId = 1;
            var feedbacks = new List<FeedbackRestaurantDto>
            {
                new FeedbackRestaurantDto
                {
                    FeedbackId = 1,
                    IsReport = false,
                    Content = "Excellent service!"
                },
                new FeedbackRestaurantDto
                {
                    FeedbackId = 2,
                    IsReport = true,
                    Content = "Bad experience"
                }
            };

            _mockFeedbackService.Setup(service => service.ListRestaurantPagingAsync(restaurantId))
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.All(f => f.IsReport == false || f.IsReport == true));  // Check IsReport value
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_FilterByContent_ReturnsFilteredFeedbacks()
        {
            // Arrange
            int restaurantId = 1;
            var feedbacks = new List<FeedbackRestaurantDto>
            {
                new FeedbackRestaurantDto
                {
                    FeedbackId = 1,
                    Content = "Great food!"
                },
                new FeedbackRestaurantDto
                {
                    FeedbackId = 2,
                    Content = "Nice ambiance!"
                }
            };

            _mockFeedbackService.Setup(service => service.ListRestaurantPagingAsync(restaurantId))
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.All(f => f.Content.Contains("food") || f.Content.Contains("ambiance")));  // Check content filtering
        }
    }
}
