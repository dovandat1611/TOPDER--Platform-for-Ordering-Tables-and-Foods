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
    public class GetAllBlogGroupsTest
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
        public async Task GetAllBlogGroups_ReturnsAllBlogGroups()
        {
            // Arrange
            var mockBlogGroups = new List<BlogGroupDto>
        {
            new BlogGroupDto { BloggroupId = 1, BloggroupName = "Tech" },
            new BlogGroupDto { BloggroupId = 2, BloggroupName = "Science" },
            new BlogGroupDto { BloggroupId = 3, BloggroupName = "Health" }
        };

            _mockBlogGroupService.Setup(service => service.GetAllBlogsAsync())
                .ReturnsAsync(mockBlogGroups);

            // Act
            var result = await _controller.GetAllBlogGroups() as OkObjectResult;
            var blogGroups = result?.Value as List<BlogGroupDto>;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(blogGroups);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, blogGroups.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tech", blogGroups[0].BloggroupName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Science", blogGroups[1].BloggroupName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Health", blogGroups[2].BloggroupName);
        }
    }
}
