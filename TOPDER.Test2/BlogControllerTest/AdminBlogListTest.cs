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
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class AdminBlogListTest
    {
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private Mock<IBlogService> _mockBlogService;
        private BlogController _controller;

        [TestInitialize]
        public void Initialize()
        {
            // Mock dependencies
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _mockBlogService = new Mock<IBlogService>();
            // Create instance of the controller with mocked dependencies
            _controller = new BlogController(_mockBlogService.Object, _mockCloudinaryService.Object);
        }

        [TestMethod]
        public async Task AdminBlogList_ValidRequest_ReturnsPaginatedResponse()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var blogGroupId = (int?)null;
            var title = (string?)null;
            var blogList = new List<BlogAdminDto>
            {
                new BlogAdminDto { BlogId = 1, Title = "Blog 1", CreateDate = DateTime.Now },
                new BlogAdminDto { BlogId = 2, Title = "Blog 2", CreateDate = DateTime.Now }
            };

            var totalCount = blogList.Count; // Total number of blogs
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Calculate total pages

            var paginatedResult = new PaginatedList<BlogAdminDto>(
                blogList,       // List of blogs
                pageNumber,     // Current page number
                totalPages,     // Total number of pages
                totalCount      // Total number of blogs
            );

            _mockBlogService.Setup(service => service.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.AdminBlogList(pageNumber, pageSize, blogGroupId, title);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<BlogAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasPreviousPage);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasNextPage);
        }

        [TestMethod]
        public async Task AdminBlogList_WithBlogGroupId_ReturnsFilteredBlogs()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var blogGroupId = 1;
            var title = (string?)null;
            var blogList = new List<BlogAdminDto>
            {
                new BlogAdminDto { BlogId = 1, BloggroupId = 1, Title = "Blog 1", CreateDate = DateTime.Now }
            };

            var totalCount = blogList.Count; // Total number of blogs
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Calculate total pages

            var paginatedResult = new PaginatedList<BlogAdminDto>(
                blogList,       // List of blogs
                pageNumber,     // Current page number
                totalPages,     // Total number of pages
                totalCount      // Total number of blogs
            );

            _mockBlogService.Setup(service => service.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.AdminBlogList(pageNumber, pageSize, blogGroupId, title);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<BlogAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items[0].BloggroupId);
        }

        [TestMethod]
        public async Task AdminBlogList_WithTitleFilter_ReturnsFilteredBlogs()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var blogGroupId = (int?)null;
            var title = "Blog 1";
            var blogList = new List<BlogAdminDto>
            {
                new BlogAdminDto { BlogId = 1, Title = "Blog 1", CreateDate = DateTime.Now }
            };

            var totalCount = blogList.Count; // Total number of blogs
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Calculate total pages

            var paginatedResult = new PaginatedList<BlogAdminDto>(
                blogList,       // List of blogs
                pageNumber,     // Current page number
                totalPages,     // Total number of pages
                totalCount      // Total number of blogs
            );

            _mockBlogService.Setup(service => service.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.AdminBlogList(pageNumber, pageSize, blogGroupId, title);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<BlogAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog 1", response.Items[0].Title);
        }

        [TestMethod]
        public async Task AdminBlogList_NoFilters_ReturnsAllBlogs()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var blogGroupId = (int?)null;
            var title = (string?)null;
            var blogList = new List<BlogAdminDto>
            {
                new BlogAdminDto { BlogId = 1, Title = "Blog 1", CreateDate = DateTime.Now },
                new BlogAdminDto { BlogId = 2, Title = "Blog 2", CreateDate = DateTime.Now }
            };

            var totalCount = blogList.Count; // Total number of blogs
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Calculate total pages

            var paginatedResult = new PaginatedList<BlogAdminDto>(
                blogList,       // List of blogs
                pageNumber,     // Current page number
                totalPages,     // Total number of pages
                totalCount      // Total number of blogs
            );

            _mockBlogService.Setup(service => service.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.AdminBlogList(pageNumber, pageSize, blogGroupId, title);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<BlogAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);
        }

        [TestMethod]
        public async Task AdminBlogList_EmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var blogGroupId = (int?)null;
            var title = "Non-existent Title";
            var blogList = new List<BlogAdminDto>();

            var totalCount = blogList.Count; // Total number of blogs
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Calculate total pages

            var paginatedResult = new PaginatedList<BlogAdminDto>(
                blogList,       // List of blogs
                pageNumber,     // Current page number
                totalPages,     // Total number of pages
                totalCount      // Total number of blogs
            );

            _mockBlogService.Setup(service => service.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.AdminBlogList(pageNumber, pageSize, blogGroupId, title);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedResponseDto<BlogAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count);
        }
    }
}
