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
        public async Task GetAdminFeedbacks_DefaultPagination_ReturnsPaginatedFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackAdminDto>(
                new List<FeedbackAdminDto> { new FeedbackAdminDto { FeedbackId = 1, Content = "Feedback content" } },
                1, 10, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListAdminPagingAsync())
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count            );
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
        }

        [TestMethod]
        public async Task GetAdminFeedbacks_FilterByStar_ReturnsFilteredFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackAdminDto>(
                new List<FeedbackAdminDto> { new FeedbackAdminDto { FeedbackId = 2, Star = 5, Content = "Excellent!" } },
                1, 1, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListAdminPagingAsync())
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Items.TrueForAll(f => f.Star == 5));
        }

        [TestMethod]
        public async Task GetAdminFeedbacks_FilterByContent_ReturnsFilteredFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackAdminDto>(
                new List<FeedbackAdminDto> { new FeedbackAdminDto { FeedbackId = 3, Content = "Good service!" } },
                1, 1, 1
            );

            _mockFeedbackService
                .Setup(service => service.ListAdminPagingAsync())
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Items.TrueForAll(f => f.Content.Contains("service")));
        }

    }
}
