﻿using Microsoft.AspNetCore.Mvc;
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
    public class GetCustomerFeedbacksTest
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
        public async Task GetCustomerFeedbacks_ValidParameters_ReturnsPaginatedResponse()
        {
            // Arrange
            var feedbackList = new List<FeedbackCustomerDto>
        {
            new FeedbackCustomerDto
            {
                FeedbackId = 1,
                CustomerId = 101,
                OrderId = 1001,
                CustomerName = "John Doe",
                CustomerImage = "john_doe.jpg",
                Star = 5,
                Content = "Great service!",
                CreateDate = DateTime.UtcNow
            },
            new FeedbackCustomerDto
            {
                FeedbackId = 2,
                CustomerId = 102,
                OrderId = 1002,
                CustomerName = "Jane Smith",
                CustomerImage = "jane_smith.jpg",
                Star = 4,
                Content = "Good food!",
                CreateDate = DateTime.UtcNow.AddDays(-1)
            }
        };

            var paginatedList = new PaginatedList<FeedbackCustomerDto>(feedbackList, feedbackList.Count, 1, 2);
            _feedbackServiceMock
                .Setup(service => service.ListCustomerPagingAsync(1, 2, 1, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetCustomerFeedbacks(1, 1, 2);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(2, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("John Doe", response.Items[0].CustomerName);
        }

        

        [TestMethod]
        public async Task GetCustomerFeedbacks_WithStarFilter_ReturnsFilteredFeedback()
        {
            // Arrange
            var feedbackList = new List<FeedbackCustomerDto>
        {
            new FeedbackCustomerDto
            {
                FeedbackId = 3,
                CustomerId = 103,
                OrderId = 1003,
                CustomerName = "Alice",
                CustomerImage = "alice.jpg",
                Star = 5,
                Content = "Excellent!",
                CreateDate = DateTime.UtcNow
            }
        };

            var paginatedList = new PaginatedList<FeedbackCustomerDto>(feedbackList, feedbackList.Count, 1, 1);

            _feedbackServiceMock
                .Setup(service => service.ListCustomerPagingAsync(1, 1, 1, 5))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetCustomerFeedbacks(1, 1, 1, 5);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, response.Items[0].Star);
        }

        [TestMethod]
        public async Task GetCustomerFeedbacks_NonRestaurant_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = 99999;
            int pageNumber = 1;
            int pageSize = 10;

            // Setup mock to return an empty paginated list when the restaurantId is 99999
            var emptyFeedbackList = new PaginatedList<FeedbackCustomerDto>(new List<FeedbackCustomerDto>(), pageNumber, pageSize, 0);
            _feedbackServiceMock
                .Setup(service => service.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, null))
                .ReturnsAsync(emptyFeedbackList);

            // Act
            var result = await _controller.GetCustomerFeedbacks(restaurantId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult, "Expected an OkObjectResult");
            var response = okResult.Value as PaginatedResponseDto<FeedbackCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response, "Expected a PaginatedResponseDto<FeedbackCustomerDto>");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count, "Expected an empty feedback list");
        }
    }
}