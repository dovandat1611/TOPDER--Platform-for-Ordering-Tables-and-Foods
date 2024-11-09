using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class GetBlogByIdTest
    {
        private Mock<ICloudinaryService> mockCloudinaryService;
        private Mock<IBlogService> mockBlogService;
        private BlogController controller;

        [TestInitialize]
        public void Initialize()
        {
            // Mock dependencies
            mockCloudinaryService = new Mock<ICloudinaryService>();
            mockBlogService = new Mock<IBlogService>();
            // Create instance of the controller with mocked dependencies
            controller = new BlogController(mockBlogService.Object, mockCloudinaryService.Object);
        }

        [TestMethod]
        public async Task GetBlogById_WhenBlogIdIsInvalid_ReturnNotFound()
        {
            // Arrange
            var invalidBlogId = -1;

            // Simulate the blog service throwing a KeyNotFoundException for the invalid ID
            mockBlogService.Setup(x => x.GetBlogByIdAsync(invalidBlogId))
                           .ThrowsAsync(new KeyNotFoundException("Blog not found"));

            // Act
            var result = await controller.GetBlogById(invalidBlogId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);  // Ensure the result is a NotFoundObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);  // Verify status code 404
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog not found", notFoundResult.Value);  // Verify the error message
        }

        [TestMethod]
        public async Task GetBlogById_WhenBlogExists_ReturnOk()
        {
            // Arrange
            int blogId = 1;
            var expectedBlogDetail = new BlogDetailDto
            {
                
            };
            mockBlogService.Setup(x => x.GetBlogByIdAsync(blogId))
                           .ReturnsAsync(expectedBlogDetail);

            // Act
            var result = await controller.GetBlogById(blogId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedBlogDetail, okResult.Value);
        }
        [TestMethod]
        public async Task GetBlogById_WhenBlogIdIsZero_ReturnNotFound()
        {
            // Arrange
            var zeroBlogId = 0;

            // Simulate the blog service throwing a KeyNotFoundException for the ID 0
            mockBlogService.Setup(x => x.GetBlogByIdAsync(zeroBlogId))
                           .ThrowsAsync(new KeyNotFoundException("Blog not found"));

            // Act
            var result = await controller.GetBlogById(zeroBlogId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);  // Ensure the result is a NotFoundObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);  // Verify the status code is 404
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog not found", notFoundResult.Value);  // Verify the error message
        }
    }
}
