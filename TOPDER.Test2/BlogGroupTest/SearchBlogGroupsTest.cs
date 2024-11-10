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
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.BlogGroupTest
{
    [TestClass]
    public class SearchBlogGroupsTest
    {
        private Mock<IBlogGroupService> _mockBlogGroupService;
        private BlogGroupController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockBlogGroupService = new Mock<IBlogGroupService>();
            _controller = new BlogGroupController(_mockBlogGroupService.Object);
        }

        [TestMethod]
        public async Task SearchBlogGroups_WithNullBlogGroupName_ReturnsPaginatedResult()
        {
            // Arrange
            int pageNumber = 1, pageSize = 10;
            var mockResponse = new PaginatedList<BlogGroupDto>(
                new List<BlogGroupDto>
                {
                new BlogGroupDto { BloggroupId = 1, BloggroupName = "Tech" },
                new BlogGroupDto { BloggroupId = 2, BloggroupName = "Science" }
                },
                2, pageNumber, pageSize
            );

            _mockBlogGroupService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, null))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.SearchBlogGroups(pageNumber, pageSize, null) as OkObjectResult;
            var paginatedResponse = result?.Value as PaginatedResponseDto<BlogGroupDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(paginatedResponse);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, paginatedResponse.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tech", paginatedResponse.Items.First().BloggroupName);
        }

        [TestMethod]
        public async Task SearchBlogGroups_WithNonNullBlogGroupName_ReturnsFilteredPaginatedResult()
        {
            // Arrange
            int pageNumber = 1, pageSize = 10;
            string blogGroupName = "Tech";
            var mockResponse = new PaginatedList<BlogGroupDto>(
                new List<BlogGroupDto>
                {
                new BlogGroupDto { BloggroupId = 1, BloggroupName = "Tech" }
                },
                1, pageNumber, pageSize
            );

            _mockBlogGroupService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, blogGroupName))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.SearchBlogGroups(pageNumber, pageSize, blogGroupName) as OkObjectResult;
            var paginatedResponse = result?.Value as PaginatedResponseDto<BlogGroupDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(paginatedResponse);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, paginatedResponse.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tech", paginatedResponse.Items.First().BloggroupName);
        }
    }
}
