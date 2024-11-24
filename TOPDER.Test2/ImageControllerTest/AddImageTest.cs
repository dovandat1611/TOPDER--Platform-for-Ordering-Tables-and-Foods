using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;
using TOPDER.Service.Dtos.Image; // Ensure you're using the correct ImageDto

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class AddImageTest
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
        public async Task AddImage_NoFiles_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddImage(1, null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có file nào để upload.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddImage_AllUploadsFail_ReturnsBadRequest()
        {
            // Arrange
            var mockFiles = new List<IFormFile> { new Mock<IFormFile>().Object, new Mock<IFormFile>().Object };
            _cloudinaryServiceMock
                .Setup(service => service.UploadImagesAsync(mockFiles))
                .ReturnsAsync(new List<ImageUploadResult>
                {
                    new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.BadRequest },
                    new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.BadRequest }
                });

            // Act
            var result = await _controller.AddImage(1, mockFiles);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có ảnh nào được tải lên thành công.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddImage_SomeUploadsSucceed_ReturnsOkWithUrls()
        {
            // Arrange
            var mockFiles = new List<IFormFile> { new Mock<IFormFile>().Object, new Mock<IFormFile>().Object };
            var uploadResults = new List<ImageUploadResult>
            {
                new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.OK, SecureUrl = new Uri("http://example.com/image1.jpg") },
                new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.BadRequest }
            };

            _cloudinaryServiceMock
                .Setup(service => service.UploadImagesAsync(mockFiles))
                .ReturnsAsync(uploadResults);
                
            // Prepare expected ImageDto list
            List<ImageDto> imageDtos = new List<ImageDto>
            {
                new ImageDto { ImageId = 0, RestaurantId = 1, ImageUrl = "http://example.com/image1.jpg" }
            };

            _imageServiceMock
                .Setup(service => service.AddRangeAsync(It.IsAny<List<ImageDto>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddImage(1, mockFiles);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task AddImage_ImageSavingFails_ReturnsServerError()
        {
            // Arrange
            var mockFiles = new List<IFormFile> { new Mock<IFormFile>().Object };
            var uploadResults = new List<ImageUploadResult>
            {
                new ImageUploadResult { StatusCode = System.Net.HttpStatusCode.OK, SecureUrl = new Uri("http://example.com/image.jpg") }
            };

            _cloudinaryServiceMock
                .Setup(service => service.UploadImagesAsync(mockFiles))
                .ReturnsAsync(uploadResults);

            // Ensure the AddRangeAsync returns false to simulate failure
            _imageServiceMock
                .Setup(service => service.AddRangeAsync(It.IsAny<List<ImageDto>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddImage(1, mockFiles);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(serverErrorResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(StatusCodes.Status500InternalServerError, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Có lỗi xảy ra khi thêm hình ảnh.", serverErrorResult.Value);
        }
    }
}
