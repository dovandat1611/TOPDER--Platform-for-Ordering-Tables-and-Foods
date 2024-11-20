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
    public class GetUpdateItemAsyncTest
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
        public async Task GetUpdateItemAsync_WhenBlogNotFound_ReturnNotFound()
        {
            // Arrange
            var invalidBlogId = 999999;  // Giả sử ID âm là không hợp lệ

            // Mô phỏng lỗi trong service hoặc controller khi blogId không tồn tại
            mockBlogService.Setup(x => x.GetUpdateItemAsync(invalidBlogId))
                           .ThrowsAsync(new KeyNotFoundException("Blog not found"));

            // Act
            var result = await controller.GetUpdateItemAsync(invalidBlogId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);  // Kiểm tra rằng kết quả là NotFound
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);  // Kiểm tra mã trạng thái là 404
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog not found", notFoundResult.Value);  // Kiểm tra thông báo lỗi
        }

        [TestMethod]
        public async Task GetUpdateItemAsync_WhenBlogExists_ReturnOk()
        {
            // Arrange
            var validBlogId = 1;  // Giả sử blogId 1 tồn tại và hợp lệ
            var expectedBlogModel = new UpdateBlogModel
            {
                BlogId = validBlogId,
                Title = "Existing Blog Title",
                Content = "Existing Blog content"
            };

            // Mô phỏng service trả về blog hợp lệ khi tìm kiếm theo ID
            mockBlogService.Setup(x => x.GetUpdateItemAsync(validBlogId))
                           .ReturnsAsync(expectedBlogModel);

            // Act
            var result = await controller.GetUpdateItemAsync(validBlogId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Kiểm tra rằng kết quả là OkResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);  // Kiểm tra mã trạng thái là 200 (OK)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedBlogModel, okResult.Value);  // Kiểm tra dữ liệu trả về là đúng
        }
    }
}
