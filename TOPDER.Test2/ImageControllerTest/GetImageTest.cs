using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;
using TOPDER.Service.Dtos.Image; // Make sure to import the correct namespace

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class GetImageTest
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
        public async Task GetImage_ValidId_ReturnsOkWithImage()
        {
            // Arrange
            int restaurantId = 1;
            int imageId = 1;

            var imageDto = new ImageDto
            {
                ImageId = imageId,
                RestaurantId = restaurantId,
                ImageUrl = "http://example.com/image.jpg"
            };

            _imageServiceMock
                .Setup(service => service.GetItemAsync(imageId, restaurantId))
                .ReturnsAsync(imageDto);

            // Act
            var result = await _controller.GetImage(restaurantId, imageId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as ImageDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(imageId, returnValue.ImageId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(restaurantId, returnValue.RestaurantId);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("http://example.com/image.jpg", returnValue.ImageUrl);
        }

        [TestMethod]
        public async Task GetImage_ImageNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int imageId = 999; // Assume imageId doesn't exist

            _imageServiceMock
                .Setup(service => service.GetItemAsync(imageId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException("Image not found"));

            // Act
            var result = await _controller.GetImage(restaurantId, imageId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Image not found", notFoundResult.Value);
        }
    }
}
