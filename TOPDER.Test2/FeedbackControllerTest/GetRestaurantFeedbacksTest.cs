using Microsoft.AspNetCore.Mvc;
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
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.FeedbackControllerTest
{
    [TestClass]
    public class GetRestaurantFeedbacksTest
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
        public async Task GetRestaurantFeedbacks_DefaultPagination_ReturnsPaginatedFeedbacks()
        {
            // Arrange
            var feedbacks = new PaginatedList<FeedbackRestaurantDto>(
                new List<FeedbackRestaurantDto> { new FeedbackRestaurantDto { FeedbackId = 1, Content = "Feedback content" } },
                1, 10, 1
            );

            _feedbackServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(1, 10, 1, null, null))
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

            _feedbackServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(1, 10, 1, 5, null))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(1, 1, 10, 5);

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

            _feedbackServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(1, 10, 1, null, "service"))
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetRestaurantFeedbacks(1, 1, 10, null, "service");

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

            _feedbackServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(1, 10, invalidRestaurantId, null, null))
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
