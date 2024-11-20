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

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class DeleteBlogTest
    {
        private Mock<ICloudinaryService> mockCloudinaryService;
        private Mock<IBlogService> mockBlogService;
        private BlogController controller;

        [TestInitialize]
        public void Initialize()
        {
            mockCloudinaryService = new Mock<ICloudinaryService>();
            mockBlogService = new Mock<IBlogService>();
            controller = new BlogController(mockBlogService.Object, mockCloudinaryService.Object);
        }

        [TestMethod]
        public async Task DeleteBlog_WhenBlogDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var invalidBlogId = 9999;  // An ID that does not exist in the system

            // Mock the RemoveAsync method to return false, simulating the blog does not exist
            mockBlogService.Setup(x => x.RemoveAsync(invalidBlogId)).ReturnsAsync(false);

            // Act
            var result = await controller.DeleteBlog(invalidBlogId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);  // Ensure the result is a NotFoundObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);  // Verify the status code is 404
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Blog với ID {invalidBlogId} không tồn tại.", notFoundResult.Value);  // Check the error message
        }

        [TestMethod]
        public async Task DeleteBlog_WhenBlogExists_ReturnsOk()
        {
            // Arrange
            var validBlogId = 1;  // A valid ID that exists in the system

            // Mock the RemoveAsync method to return true, simulating the blog was deleted
            mockBlogService.Setup(x => x.RemoveAsync(validBlogId)).ReturnsAsync(true);

            // Act
            var result = await controller.DeleteBlog(validBlogId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure the result is an OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);  // Verify the status code is 200
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Xóa Blog với ID {validBlogId} thành công.", okResult.Value);  // Check the success message
        }

    }
}
