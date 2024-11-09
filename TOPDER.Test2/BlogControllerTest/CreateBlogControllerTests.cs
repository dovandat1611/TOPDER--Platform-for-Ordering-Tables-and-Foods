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

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class CreateBlogControllerTests
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
        public async Task CreateBlog_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            // Arrange
            var invalidBlogModel = new CreateBlogModel
            {
                Title = "", // Invalid title
                Content = "Content"
            };
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.CreateBlog(invalidBlogModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateBlog_WhenNoImageUploaded_ReturnOk()
        {
            // Arrange
            var blogModel = new CreateBlogModel
            {
                Title = "Blog Title",
                Content = "Blog content",
                ImageFile = null // No image uploaded
            };

            // Mock the AddAsync method to return true (success)
            mockBlogService.Setup(x => x.AddAsync(It.IsAny<CreateBlogModel>())).ReturnsAsync(true);

            // Act
            var result = await controller.CreateBlog(blogModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Blog thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task CreateBlog_WhenImageUploaded_ReturnOk()
        {
            // Arrange
            var blogModel = new CreateBlogModel
            {
                Title = "Blog Title",
                Content = "Blog content",
                ImageFile = new FormFile(null, 0, 0, "file", "image.jpg") // Mock image file
            };
            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("http://test.com/image.jpg")
            };

            // Mock the Cloudinary service to return the image URL
            mockCloudinaryService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            // Mock the AddAsync method to return true (success)
            mockBlogService.Setup(x => x.AddAsync(It.IsAny<CreateBlogModel>())).ReturnsAsync(true);

            // Act
            var result = await controller.CreateBlog(blogModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Blog thành công.", okResult.Value);
        }
        [TestMethod]
        public async Task CreateBlog_WhenImageUploadFails_ReturnInternalServerError()
        {
            // Arrange
            var blogModel = new CreateBlogModel
            {
                Title = "Blog Title",
                Content = "Blog content",
                ImageFile = new FormFile(null, 0, 0, "file", "image.jpg")
            };

            // Mock the Cloudinary service to simulate a failed image upload (returns null)
            mockCloudinaryService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((ImageUploadResult)null);  // Simulate failure by returning null

            // Act
            var result = await controller.CreateBlog(blogModel);

            // Assert
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(objectResult);  // Ensure we got an ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);  // Check the status code is 500
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Có lỗi xảy ra khi thêm bài viết.", objectResult.Value);  // Check the error message
        }
        [TestMethod]
        public async Task CreateBlog_WhenImageIsNull_ReturnInternalServerError()
        {
            // Arrange
            var blogModel = new CreateBlogModel
            {
                Title = "Blog Title",
                Content = "Blog content",
                ImageFile = null // No image uploaded
            };

            // Act
            var result = await controller.CreateBlog(blogModel);

            // Assert
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(objectResult);  // Ensure we got an ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);  // Check that the status code is 500
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Có lỗi xảy ra khi thêm bài viết.", objectResult.Value);  // Ensure the error message is correct
        }

        [TestMethod]
        public async Task CreateBlog_WhenContentIsNull_ReturnBadRequest()
        {
            // Arrange
            var invalidBlogModel = new CreateBlogModel
            {
                Title = "TIdsle", // Invalid title
                Content = ""
            };
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.CreateBlog(invalidBlogModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }

}
