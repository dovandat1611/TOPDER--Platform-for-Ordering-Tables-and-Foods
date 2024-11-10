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
    public class GetExistingBlogGroupsTest
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
        public async Task GetExistingBlogGroups_ReturnsOkWithBlogGroups()
        {
            // Arrange
            var blogGroups = new List<BlogGroupDto>
        {
            new BlogGroupDto { /* initialize properties */ },
            new BlogGroupDto { /* initialize properties */ }
        };
            _mockBlogGroupService.Setup(service => service.BlogGroupExistAsync()).ReturnsAsync(blogGroups);

            // Act
            var result = await _controller.GetExistingBlogGroups();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(blogGroups, okResult.Value);
        }

        [TestMethod]
        public async Task GetExistingBlogGroups_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockBlogGroupService.Setup(service => service.BlogGroupExistAsync()).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetExistingBlogGroups();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var errorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, errorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đã xảy ra lỗi trong quá trình xử lý.", errorResult.Value);
        }
    }

}
