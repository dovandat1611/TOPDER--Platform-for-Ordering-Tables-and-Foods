using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class GetAdminFeedbacksTest
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
        public async Task GetAdminFeedbacks_ReturnsOkResult_WithFeedbackList()
        {
            // Arrange
            var feedbacks = new List<FeedbackAdminDto>
            {
                new FeedbackAdminDto
                {
                    FeedbackId = 1,
                    CustomerId = 1,
                    OrderId = 1,
                    CustomerName = "John Doe",
                    RestaurantId = 1,
                    RestaurantName = "Restaurant A",
                    Star = 5,
                    Content = "Excellent!",
                    CreateDate = DateTime.Now,
                    Status = "Active"
                },
                new FeedbackAdminDto
                {
                    FeedbackId = 2,
                    CustomerId = 2,
                    OrderId = 2,
                    CustomerName = "Jane Doe",
                    RestaurantId = 1,
                    RestaurantName = "Restaurant B",
                    Star = 4,
                    Content = "Very good",
                    CreateDate = DateTime.Now,
                    Status = "Active"
                }
            };

            // Mocking the service method to return the list of feedbacks
            _mockFeedbackService.Setup(service => service.ListAdminPagingAsync())
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks() as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var returnedFeedbacks = result.Value as List<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedFeedbacks);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedFeedbacks.Count);
        }

        [TestMethod]
        public async Task GetAdminFeedbacks_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            var feedbacks = new List<FeedbackAdminDto>(); // Empty list

            // Mocking the service method to return an empty list
            _mockFeedbackService.Setup(service => service.ListAdminPagingAsync())
                                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks() as OkObjectResult;

            // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var returnedFeedbacks = result.Value as List<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedFeedbacks);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, returnedFeedbacks.Count);
        }

    }
}
