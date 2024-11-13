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
    public class GetCustomerFeedbacksTest
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private FeedbackController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        // Test case for valid restaurantId and pagination
        [TestMethod]
        public async Task GetCustomerFeedbacks_WithValidRestaurantId_ReturnsOkWithFeedbackList()
        {
            // Arrange
            var restaurantId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            var feedbackList = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto { OrderId = 101, CustomerId = 1, Star = 5, Content = "Excellent food!" },
                new FeedbackCustomerDto { OrderId = 102, CustomerId = 2, Star = 4, Content = "Good service." }
            };

            // Use synchronous PaginatedList method instead of async version for in-memory list
            var paginatedList = PaginatedList<FeedbackCustomerDto>.Create(feedbackList.AsQueryable(), pageNumber, pageSize);

            // Mock the service to return paginated feedbacks
            _mockFeedbackService.Setup(service => service.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, null))
                .Returns(Task.FromResult(paginatedList));

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId, pageNumber, pageSize) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);  // Assert that the correct number of feedbacks are returned
        }

        // Test case for invalid restaurantId (non-existent)
        [TestMethod]
        public async Task GetCustomerFeedbacks_WithInvalidRestaurantId_ReturnsOkWithEmptyList()
        {
            // Arrange
            var restaurantId = 9999; // Non-existent restaurant ID
            var pageNumber = 1;
            var pageSize = 10;
            var feedbackList = new List<FeedbackCustomerDto>();

            // Use synchronous PaginatedList method instead of async version for in-memory list
            var paginatedList = PaginatedList<FeedbackCustomerDto>.Create(feedbackList.AsQueryable(), pageNumber, pageSize);

            // Mock the service to return empty list for invalid restaurantId
            _mockFeedbackService.Setup(service => service.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, null))
                .Returns(Task.FromResult(paginatedList));

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId, pageNumber, pageSize) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count);  // Assert that no feedbacks are returned
        }

        // Test case for restaurantId with specific star filter
        [TestMethod]
        public async Task GetCustomerFeedbacks_WithStarFilter_ReturnsOkWithFilteredFeedbackList()
        {
            // Arrange
            var restaurantId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            var starFilter = 5;
            var feedbackList = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto { OrderId = 101, CustomerId = 1, Star = 5, Content = "Excellent food!" },
                new FeedbackCustomerDto { OrderId = 102, CustomerId = 2, Star = 4, Content = "Good service." }
            };

            // Filter the feedbacks based on the starFilter
            var filteredFeedbacks = feedbackList.Where(f => f.Star == starFilter).ToList();

            // Use synchronous PaginatedList method instead of async version for in-memory list
            var paginatedList = PaginatedList<FeedbackCustomerDto>.CreateAsync(filteredFeedbacks.AsQueryable(), pageNumber, pageSize);

            // Mock the service to return filtered feedbacks based on star filter
            _mockFeedbackService.Setup(service => service.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, starFilter))
                .Returns(await Task.FromResult(paginatedList));

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId, pageNumber, pageSize, starFilter) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);  // Assert that only 1 feedback with star 5 is returned
        }

        // Test case for restaurantId with star null filter
        [TestMethod]
        public async Task GetCustomerFeedbacks_WithStarNull_ReturnsOkWithAllFeedbacks()
        {
            // Arrange
            var restaurantId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            int? starFilter = null;  // star is null, meaning no filtering on star rating
            var feedbackList = new List<FeedbackCustomerDto>
            {
                new FeedbackCustomerDto { OrderId = 101, CustomerId = 1, Star = 5, Content = "Excellent food!" },
                new FeedbackCustomerDto { OrderId = 102, CustomerId = 2, Star = 4, Content = "Good service." }
            };

            // Use synchronous PaginatedList method instead of async version for in-memory list
            var paginatedList = PaginatedList<FeedbackCustomerDto>.CreateAsync(feedbackList.AsQueryable(), pageNumber, pageSize);

            // Mock the service to return all feedbacks when star is null
            _mockFeedbackService.Setup(service => service.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, starFilter))
                .Returns(await Task.FromResult(paginatedList));

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId, pageNumber, pageSize, starFilter) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);  // Assert that all feedbacks are returned
        }
    }
}
