using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryRestaurantControllerTest
{
    [TestClass]
    public class GetByIdTest
    {
        private Mock<ICategoryRestaurantService> _mockCategoryRestaurantService;
        private CategoryRestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRestaurantService = new Mock<ICategoryRestaurantService>();
            _controller = new CategoryRestaurantController(_mockCategoryRestaurantService.Object);
        }

        // Kiểm tra trường hợp id hợp lệ
        [TestMethod]
        public async Task GetCategoryRestaurant_WithValidId_ReturnsOk()
        {
            // Arrange
            int categoryRestaurantId = 1; // Id hợp lệ
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = 1,
                CategoryRestaurantName = "Italian Cuisine"
            };

            _mockCategoryRestaurantService.Setup(service => service.UpdateItemAsync(categoryRestaurantId))
                .ReturnsAsync(categoryRestaurantDto); // Giả lập trả về thông tin Category Restaurant

            // Act
            var result = await _controller.GetById(categoryRestaurantId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Kiểm tra mã trạng thái HTTP
            var response = result.Value as CategoryRestaurantDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response); // Kiểm tra dữ liệu trả về
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Italian Cuisine", response.CategoryRestaurantName); // Kiểm tra tên Category Restaurant
        }

        
        // Kiểm tra trường hợp id không hợp lệ
        [TestMethod]
        public async Task GetCategoryRestaurant_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int categoryRestaurantId = -1; // Id không hợp lệ
            _mockCategoryRestaurantService.Setup(service => service.UpdateItemAsync(categoryRestaurantId))
                .ThrowsAsync(new KeyNotFoundException("Không tìm thấy Category Restaurant với ID này.")); // Giả lập KeyNotFoundException

            // Act
            var result = await _controller.GetById(categoryRestaurantId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode); // Kiểm tra mã trạng thái HTTP là 404
            var response = result.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
        }
    }
}
