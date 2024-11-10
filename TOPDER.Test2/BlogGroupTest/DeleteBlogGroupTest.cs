using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BlogGroupTest
{
    [TestClass]
    public class DeleteBlogGroupTest
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
        public async Task DeleteBlogGroup_WithValidId_ReturnsOk()
        {
            // Arrange
            int id = 1;
            _mockBlogGroupService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBlogGroup(id);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Xóa Blog group với ID {id} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task DeleteBlogGroup_WithIdZero_ReturnsNotFound()
        {
            // Arrange
            int id = 0;
            _mockBlogGroupService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBlogGroup(id);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("BlogGroup với ID 0 không tồn tại hoặc không thể xóa.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteBlogGroup_WithNegativeId_ReturnsNotFound()
        {
            // Arrange
            int id = -1;
            _mockBlogGroupService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBlogGroup(id);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("BlogGroup với ID -1 không tồn tại hoặc không thể xóa.", notFoundResult.Value);
        }
    }
}
