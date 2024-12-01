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
        public async Task GetRestaurantFeedbacks_DefaultPagination_ReturnsPaginatedFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackRestaurantDto>(
                new List<FeedbackRestaurantDto> { new FeedbackRestaurantDto { FeedbackId = 1, Content = "Feedback content" } },
                1, 10, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListRestaurantPagingAsync(1))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(1);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_FilterByStar_ReturnsFilteredFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackRestaurantDto>(
                new List<FeedbackRestaurantDto> { new FeedbackRestaurantDto { FeedbackId = 2, Star = 5, Content = "Excellent!" } },
                1, 1, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListRestaurantPagingAsync(1))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(1);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Items.TrueForAll(f => f.Star == 5));
        }

        [TestMethod]
        public async Task GetRestaurantFeedbacks_FilterByContent_ReturnsFilteredFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackRestaurantDto>(
                new List<FeedbackRestaurantDto> { new FeedbackRestaurantDto { FeedbackId = 3, Content = "Great service!" } },
                1, 1, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListRestaurantPagingAsync(1))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(1 );

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.IsTrue(response.Items.TrueForAll(f => f.Content.Contains("service")));
        }

        


        [TestMethod]
        public async Task GetRestaurantFeedbacks_InvalidRestaurantId_ReturnsEmptyResponse()
        {
            // Arrange
            int invalidRestaurantId = 99999;
            var emptyFeedbackList = new PaginatedList<FeedbackRestaurantDto>(new List<FeedbackRestaurantDto>(), 1, 0, 1);

            _mockFeedbackService
                .Setup(service => service.ListRestaurantPagingAsync( invalidRestaurantId))
                .ReturnsAsync(emptyFeedbackList);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(invalidRestaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count); // Đảm bảo danh sách trống
        }   
    }
}
