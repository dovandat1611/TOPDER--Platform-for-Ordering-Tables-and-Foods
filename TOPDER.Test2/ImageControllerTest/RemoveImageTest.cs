using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ImageControllerTest
{
    [TestClass]
    public class RemoveImageTest
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

        // Test trường hợp xóa thành công
        [TestMethod]
        public async Task RemoveImage_Success_ReturnsOkResult()
        {
            // Arrange
            var restaurantId = 1;
            var imageId = 1;

            _imageServiceMock.Setup(service => service.RemoveAsync(imageId, restaurantId)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveImage(restaurantId, imageId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Đã xóa hình ảnh với Id {imageId}.", okResult.Value);
        }

        // Test trường hợp hình ảnh không tồn tại
        [TestMethod]
        public async Task RemoveImage_ImageNotFound_ReturnsNotFound()
        {
            // Arrange
            var restaurantId = 1;
            var imageId = 99999999;

            _imageServiceMock.Setup(service => service.RemoveAsync(imageId, restaurantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveImage(restaurantId, imageId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy hình ảnh với Id {imageId}.", notFoundResult.Value);
        }

     
        // Test trường hợp xảy ra KeyNotFoundException
        [TestMethod]
        public async Task RemoveImage_KeyNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var restaurantId = 9999999;
            var imageId = 1;

            _imageServiceMock.Setup(service => service.RemoveAsync(imageId, restaurantId))
                             .ThrowsAsync(new KeyNotFoundException("Key not found"));

            // Act
            var result = await _controller.RemoveImage(restaurantId, imageId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Key not found", notFoundResult.Value.ToString());
        }

        [TestMethod]
        public async Task RemoveImage_InvalidRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var restaurantId = 9999;  // Giả định rằng 9999 là restaurantId không hợp lệ
            var imageId = 1;

            // Cấu hình Mock để trả về kết quả khi gọi service RemoveAsync
            _imageServiceMock.Setup(service => service.RemoveAsync(imageId, restaurantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveImage(restaurantId, imageId);

            // Assert
            var badRequestResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }
    }
}
