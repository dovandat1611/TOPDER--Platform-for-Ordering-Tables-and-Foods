using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BlogGroupTest
{
    [TestClass]
    public class GetBlogGroupTest
    {
        private BlogGroupController _controller;
        private Mock<IBlogGroupService> _mockBlogGroupService;

        [TestInitialize]
        public void SetUp()
        {
            _mockBlogGroupService = new Mock<IBlogGroupService>();
            _controller = new BlogGroupController(_mockBlogGroupService.Object);
        }

        [TestMethod]
        public async Task GetBlogGroup_WithValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1;
            var blogGroupDto = new BlogGroupDto(); // Assuming BlogGroupDto is your DTO model
            _mockBlogGroupService.Setup(service => service.GetItemAsync(validId)).ReturnsAsync(blogGroupDto);

            // Act
            var result = await _controller.GetBlogGroup(validId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(blogGroupDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetBlogGroup_WithIdZero_ReturnsNotFound()
        {
            // Arrange
            int invalidId = 0;
            _mockBlogGroupService.Setup(service => service.GetItemAsync(invalidId)).ThrowsAsync(new KeyNotFoundException("Blog group not found"));

            // Act
            var result = await _controller.GetBlogGroup(invalidId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog group not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetBlogGroup_WithIdNegative_ReturnsInternalServerError()
        {
            // Arrange
            int negativeId = 1;
            _mockBlogGroupService.Setup(service => service.GetItemAsync(negativeId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetBlogGroup(negativeId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var errorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, errorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đã xảy ra lỗi trong quá trình xử lý: Unexpected error", errorResult.Value);
        }
    }
}
