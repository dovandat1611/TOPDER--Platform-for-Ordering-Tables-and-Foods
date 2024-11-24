using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class GetAdminFeedbacksTest
    {
        private Mock<IFeedbackService> _feedbackServiceMock;
        private FeedbackController _controller;

        [TestInitialize]
        public void Setup()
        {
            _feedbackServiceMock = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_feedbackServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAdminFeedbacks_DefaultPagination_ReturnsPaginatedFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackAdminDto>(
                new List<FeedbackAdminDto> { new FeedbackAdminDto { FeedbackId = 1, Content = "Feedback content" } },
                1, 10, 1
            );

            _feedbackServiceMock
                .Setup(service => service.ListAdminPagingAsync(1, 10, null, null))
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

            _feedbackServiceMock
                .Setup(service => service.ListAdminPagingAsync(1, 10, 5, null))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks(1, 10, 5);

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

            _feedbackServiceMock
                .Setup(service => service.ListAdminPagingAsync(1, 10, null, "service"))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAdminFeedbacks(1, 10, null, "service");

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(response.Items.TrueForAll(f => f.Content.Contains("service")));
        }

    }
}
