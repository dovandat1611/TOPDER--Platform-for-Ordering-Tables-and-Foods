using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.FeedbackControllerTest
{
    public class GetHistoryCustomerPagingTest
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private FeedbackController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _controller = new FeedbackController(_mockFeedbackService.Object);
        }

        // Test case for customerId = 1 (Valid customer ID with feedback history)
        [TestMethod]
        public async Task GetHistoryCustomerPaging_WithValidCustomerId_ReturnsOkWithFeedbackHistory()
        {
            // Arrange
            var customerId = 1;
            var pageNumber = 1;
            var pageSize = 10;
            var feedbackHistory = new List<FeedbackHistoryDto>
            {
                new FeedbackHistoryDto {  RestaurantId= 1, OrderId = 101, Star = 5, Content = "Great food!" },
                new FeedbackHistoryDto {RestaurantId = 2, OrderId = 102,  Star = 4, Content = "Good service." }
            };

            // Tạo một PaginatedList để trả về
            var paginatedList = PaginatedList<FeedbackHistoryDto>.CreateAsync(
                feedbackHistory.AsQueryable(),
                pageNumber,
                pageSize
            ).Result;

            _mockFeedbackService.Setup(service => service.GetHistoryCustomerPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedList); // Mô phỏng lịch sử feedback cho customerId = 1

            // Act
            var result = await _controller.GetHistoryCustomerPaging(pageNumber, pageSize, customerId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedList<FeedbackHistoryDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count); // Assert có 2 feedbacks
        }

        // Test case for customerId = 9999 (Non-existent customer ID, should return empty list)
        [TestMethod]
        public async Task GetHistoryCustomerPaging_WithNonExistentCustomerId_ReturnsOkWithEmptyList()
        {
            // Arrange
            var customerId = 9999;
            var pageNumber = 1;
            var pageSize = 10;
            var feedbackHistory = new List<FeedbackHistoryDto>();

            // Tạo một PaginatedList rỗng
            var paginatedList = PaginatedList<FeedbackHistoryDto>.CreateAsync(
                feedbackHistory.AsQueryable(),
                pageNumber,
                pageSize
            ).Result;

            _mockFeedbackService.Setup(service => service.GetHistoryCustomerPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedList); // Simulate no feedback history for customerId = 9999

            // Act
            var result = await _controller.GetHistoryCustomerPaging(pageNumber, pageSize, customerId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedList<FeedbackHistoryDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0,response.Count); // Assert danh sách rỗng
        }

        // Test case for customerId = -1 (Invalid customer ID, should return empty list)
        [TestMethod]
        public async Task GetHistoryCustomerPaging_WithInvalidCustomerId_ReturnsOkWithEmptyList()
        {
            // Arrange
            var customerId = -1;
            var pageNumber = 1;
            var pageSize = 10;
            var feedbackHistory = new List<FeedbackHistoryDto>();

            // Tạo một PaginatedList rỗng
            var paginatedList = PaginatedList<FeedbackHistoryDto>.CreateAsync(
                feedbackHistory.AsQueryable(),
                pageNumber,
                pageSize
            ).Result;

            _mockFeedbackService.Setup(service => service.GetHistoryCustomerPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedList); // Simulate no feedback history for customerId = -1

            // Act
            var result = await _controller.GetHistoryCustomerPaging(pageNumber, pageSize, customerId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as PaginatedList<FeedbackHistoryDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Count); // Assert danh sách rỗng
        }
    }
}
