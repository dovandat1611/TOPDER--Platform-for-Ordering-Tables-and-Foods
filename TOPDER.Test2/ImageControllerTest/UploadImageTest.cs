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
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class UploadImageTest
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
        public async Task UploadImage_ValidFile_ReturnsOkWithImageUrl()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1); // File is not empty
            var uploadResult = new ImageUploadResult { SecureUrl = new Uri("http://example.com/image.jpg") };

            _cloudinaryServiceMock
                .Setup(service => service.UploadImageAsync(mockFile.Object))
                .ReturnsAsync(uploadResult);

            // Act
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult, "Expected an OkObjectResult");
        }

        [TestMethod]
        public async Task UploadImage_NullFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadImage(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult");
            dynamic response = badRequestResult.Value;
        }

        [TestMethod]
        public async Task UploadImage_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0); // File is empty

            // Act
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Expected a BadRequestObjectResult");
            dynamic response = badRequestResult.Value;
        }

        [TestMethod]
        public async Task UploadImage_UploadFails_ReturnsInternalServerError()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1); // File is not empty

            _cloudinaryServiceMock
                .Setup(service => service.UploadImageAsync(mockFile.Object))
                .ReturnsAsync((ImageUploadResult)null); // Simulate upload failure

            // Act
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult, "Expected an ObjectResult with StatusCode 500");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            dynamic response = internalServerErrorResult.Value;
        }
    }

}
