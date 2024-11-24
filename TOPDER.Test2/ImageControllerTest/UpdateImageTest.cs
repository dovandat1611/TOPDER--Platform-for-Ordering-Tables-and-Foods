using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;
using CloudinaryDotNet.Actions;
using TOPDER.Service.Dtos.Image;

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class UpdateImageTest
    {
        private Mock<IImageService> _imageServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private ImageController _controller;

        [TestInitialize]
        public void Setup()
        {
            _imageServiceMock = new Mock<IImageService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new ImageController(_imageServiceMock.Object, _cloudinaryServiceMock.Object);
        }

        [TestMethod]
        public async Task UpdateImage_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var imageId = 1;
            var restaurantId = 1;
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Set a valid file length

            // Create a mock result for ImageUploadResult
            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("http://example.com/image.jpg")
            };

            // Mock the UploadImageAsync method to return the mock result
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object))
                .ReturnsAsync(uploadResult);

            _imageServiceMock.Setup(service => service.UpdateAsync(It.IsAny<ImageDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateImage(imageId, restaurantId, file.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật hình ảnh thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateImage_NullFile_ReturnsBadRequest()
        {
            // Arrange
            var imageId = 1;
            var restaurantId = 1;
            IFormFile file = null;

            // Act
            var result = await _controller.UpdateImage(imageId, restaurantId, file);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task UpdateImage_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var imageId = 1;
            var restaurantId = 1;
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(0); // Empty file

            // Act
            var result = await _controller.UpdateImage(imageId, restaurantId, file.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task UpdateImage_FailedUpload_ReturnsInternalServerError()
        {
            // Arrange
            var imageId = 1;
            var restaurantId = 1;
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            // Mock the upload failure by returning null
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object)).ReturnsAsync((ImageUploadResult)null);

            // Act
            var result = await _controller.UpdateImage(imageId, restaurantId, file.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, internalServerErrorResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateImage_ImageNotFound_ReturnsNotFound()
        {
            // Arrange
            var imageId = -1;
            var restaurantId = 1;
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1); // Valid file length

            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("http://example.com/image.jpg")
            };
            _cloudinaryServiceMock.Setup(service => service.UploadImageAsync(file.Object)).ReturnsAsync(uploadResult);

            // Mock that the image service cannot find the image
            _imageServiceMock.Setup(service => service.UpdateAsync(It.IsAny<ImageDto>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateImage(imageId, restaurantId, file.Object);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy hình ảnh với Id {imageId}.", notFoundResult.Value);
        }
    }
}
