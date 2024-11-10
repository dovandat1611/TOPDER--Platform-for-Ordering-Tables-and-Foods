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
    public class UpdateBlogGroupTest
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
        public async Task UpdateBlogGroup_WithIdZero_ReturnsNotFound()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto { BloggroupId = 0, BloggroupName = "Sample Group" };
            _mockBlogGroupService.Setup(service => service.UpdateAsync(blogGroupDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBlogGroup(blogGroupDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog Group với ID 0 không tồn tại.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlogGroup_WithIdOne_ReturnsOk()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto { BloggroupId = 1, BloggroupName = "Updated Group" };
            _mockBlogGroupService.Setup(service => service.UpdateAsync(blogGroupDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBlogGroup(blogGroupDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật Blog Group với ID 1 thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlogGroup_WithIdNegative_ReturnsNotFound()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto { BloggroupId = -1, BloggroupName = "Sample Group" };
            _mockBlogGroupService.Setup(service => service.UpdateAsync(blogGroupDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBlogGroup(blogGroupDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog Group với ID -1 không tồn tại.", notFoundResult.Value);
        }
        [TestMethod]
        public async Task UpdateBlogGroup_WithBlogGroupNameNull_ReturnsOk()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto { BloggroupId = 1, BloggroupName = null! };
            _mockBlogGroupService.Setup(service => service.UpdateAsync(blogGroupDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBlogGroup(blogGroupDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật Blog Group với ID {blogGroupDto.BloggroupId} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlogGroup_WithValidBlogGroupName_ReturnsOk()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto { BloggroupId = 1, BloggroupName = "Valid Group Name" };
            _mockBlogGroupService.Setup(service => service.UpdateAsync(blogGroupDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBlogGroup(blogGroupDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật Blog Group với ID 1 thành công.", okResult.Value);
        }
    }
}
