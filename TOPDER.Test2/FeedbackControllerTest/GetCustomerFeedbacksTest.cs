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
    public class GetCustomerFeedbacksTest
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
        public async Task GetCustomerFeedbacks_ValidRestaurantId_ReturnsFeedbacks()
        {
            // Arrange
            int restaurantId = 1; // Example restaurantId
            var feedbacks = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto
                {
                    FeedbackId = 1,
                    CustomerId = 123,
                    OrderId = 456,
                    CustomerName = "John Doe",
                    CustomerImage = "john_image_url",
                    Star = 5,
                    Content = "Great restaurant!",
                    CreateDate = DateTime.UtcNow,
                    isReply = true,
                },
                new FeedbackCustomerDto
                {
                    FeedbackId = 2,
                    CustomerId = 789,
                    OrderId = 1011,
                    CustomerName = "Jane Smith",
                    CustomerImage = "jane_image_url",
                    Star = 4,
                    Content = "Nice ambiance!",
                    CreateDate = DateTime.UtcNow,
                    isReply = false,
                    FeedbackReplyCustomer = new FeedbackReplyCustomerDto()
                }
            };

            _mockFeedbackService
                .Setup(service => service.ListCustomerPagingAsync(restaurantId))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count); // Ensure two feedbacks are returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("John Doe", response[0].CustomerName); // Check customer name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, response[0].Star); // Check star rating
        }

        [TestMethod]
        public async Task GetCustomerFeedbacks_InvalidRestaurantId_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = 99999; // Non-existent restaurantId
            var feedbacks = new List<FeedbackCustomerDto>(); // Empty list for invalid restaurantId

            _mockFeedbackService
                .Setup(service => service.ListCustomerPagingAsync(restaurantId))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Count); // Should return an empty list
        }

        [TestMethod]
        public async Task GetCustomerFeedbacks_FilterByStar_ReturnsFilteredFeedbacks()
        {
            // Arrange
            int restaurantId = 1; // Example restaurantId
            var feedbacks = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto
                {
                    FeedbackId = 1,
                    Star = 5,
                    Content = "Excellent service!"
                },
                new FeedbackCustomerDto
                {
                    FeedbackId = 2,
                    Star = 4,
                    Content = "Good experience"
                }
            };

            _mockFeedbackService
                .Setup(service => service.ListCustomerPagingAsync(restaurantId))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.All(f => f.Star >= 4)); // Ensure all feedbacks have a star >= 4
        }

        [TestMethod]
        public async Task GetCustomerFeedbacks_FilterByContent_ReturnsFilteredFeedbacks()
        {
            // Arrange
            int restaurantId = 1; // Example restaurantId
            var feedbacks = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto
                {
                    FeedbackId = 1,
                    Content = "Great food!"
                },
                new FeedbackCustomerDto
                {
                    FeedbackId = 2,
                    Content = "Nice ambiance!"
                }
            };

            _mockFeedbackService
                .Setup(service => service.ListCustomerPagingAsync(restaurantId))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as List<FeedbackCustomerDto>;
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.All(f => f.Content.Contains("food") || f.Content.Contains("ambiance"))); // Check content filtering
        }
    }
}
