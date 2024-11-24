using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class GetPagingImagesTest
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
        public async Task GetPagingImages_ValidRequest_ReturnsOkResultWithImages()
        {
            // Arrange
            var restaurantId = 1;
            var pageNumber = 1;
            var pageSize = 10;

            var mockImages = new PaginatedList<ImageDto>(
                new List<ImageDto> { new ImageDto { ImageId = 1, RestaurantId = restaurantId, ImageUrl = "http://example.com/image.jpg" } },
                1,  // Total count (1 image)
                pageNumber,  // Page index (1)
                pageSize  // Page size (10)
            );

            _imageServiceMock
                .Setup(service => service.GetPagingAsync(pageNumber, pageSize, restaurantId))
                .ReturnsAsync(mockImages);

            // Act
            var result = await _controller.GetPagingImages(restaurantId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedList<ImageDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Count);
        }

        [TestMethod]
        public async Task GetPagingImages_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var restaurantId = 1;
            var pageNumber = 1;
            var pageSize = 10;

            _imageServiceMock
                .Setup(service => service.GetPagingAsync(pageNumber, pageSize, restaurantId))
                .ThrowsAsync(new Exception("Error retrieving images"));

            // Act
            var result = await _controller.GetPagingImages(restaurantId, pageNumber, pageSize);

            // Assert
            var errorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(errorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, errorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Lỗi khi lấy dữ liệu: Error retrieving images", errorResult.Value);
        }


        [TestMethod]
        public async Task GetPagingImages_NonRestaurant_ReturnsEmptyList()
        {
            // Arrange
            var restaurantId = 9999;
            var pageNumber = 1;
            var pageSize = 10;

            var mockImages = new PaginatedList<ImageDto>(
                new List<ImageDto>(), // Empty list for restaurantId = 9999
                0,  // Total count (no images)
                pageNumber,  // Page index
                pageSize  // Page size
            );

            _imageServiceMock
                .Setup(service => service.GetPagingAsync(pageNumber, pageSize, restaurantId))
                .ReturnsAsync(mockImages);

            // Act
            var result = await _controller.GetPagingImages(restaurantId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as PaginatedList<ImageDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Count); // No images for restaurantId = 9999
        }
    }
}
