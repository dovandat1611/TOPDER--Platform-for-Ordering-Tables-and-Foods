using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
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
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class UpdateBlogTest
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
        public async Task UpdateBlog_WhenModelStateIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var invalidBlogModel = new UpdateBlogModel
            {
                BlogId = 1,
                Title = "", // Invalid title
                Content = "Content"
            };
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.UpdateBlog(invalidBlogModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBlog_WhenBlogNotFound_ReturnsNotFound()
        {
            // Arrange
            var updateBlogModel = new UpdateBlogModel
            {
                BlogId = 999, // Nonexistent blog ID
                Title = "Valid Title",
                Content = "Content"
            };

            mockBlogService.Setup(x => x.UpdateAsync(updateBlogModel)).ReturnsAsync(false);

            // Act
            var result = await controller.UpdateBlog(updateBlogModel);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Blog với ID {updateBlogModel.BlogId} không tồn tại.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlog_WhenUpdateSuccessful_ReturnsOk()
        {
            // Arrange
            var updateBlogModel = new UpdateBlogModel
            {
                BlogId = 1,
                Title = "Valid Title",
                Content = "Content",
                ImageFile = null // No image upload
            };

            mockBlogService.Setup(x => x.UpdateAsync(updateBlogModel)).ReturnsAsync(true);

            // Act
            var result = await controller.UpdateBlog(updateBlogModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Cập nhật Blog với ID {updateBlogModel.BlogId} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlog_WhenImageFileIsNull_ReturnsOk()
        {
            // Arrange
            var updateBlogModel = new UpdateBlogModel
            {
                BlogId = 1,
                Title = "Valid Title",
                Content = "Content",
                ImageFile = null // ImageFile is null
            };

            mockBlogService.Setup(x => x.UpdateAsync(updateBlogModel)).ReturnsAsync(true);

            // Act
            var result = await controller.UpdateBlog(updateBlogModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Cập nhật Blog với ID {updateBlogModel.BlogId} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateBlog_WhenBlogIdIsInvalid_ReturnsNotFound()
        {
            // Arrange
            var updateBlogModel = new UpdateBlogModel
            {
                BlogId = 0, // Invalid BlogId, assumed not to exist in database
                Title = "Valid Title",
                Content = "Content"
            };

            // Simulate that the update operation fails because the BlogId does not exist
            mockBlogService.Setup(x => x.UpdateAsync(updateBlogModel)).ReturnsAsync(false);

            // Act
            var result = await controller.UpdateBlog(updateBlogModel);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); // Check that the result is a NotFound result
            Assert.AreEqual(404, notFoundResult.StatusCode); // Check that the status code is 404 (Not Found)
            Assert.AreEqual($"Blog với ID {updateBlogModel.BlogId} không tồn tại.", notFoundResult.Value); // Check error message
        }

        [TestMethod]
        public async Task UpdateBlog_WhenContentIsNull_ReturnsBadRequest()
        {
            // Arrange
            var updateBlogModel = new UpdateBlogModel
            {
                BlogId = 1,  // Assume a valid BlogId
                Title = "Valid Title",
                Content = null // Null content, should cause model state to be invalid
            };

            // Manually add a model state error for Content being null
            controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await controller.UpdateBlog(updateBlogModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);  // Ensure the result is a BadRequest
            Assert.AreEqual(400, badRequestResult.StatusCode);  // Verify the status code is 400
        }

    }
}

