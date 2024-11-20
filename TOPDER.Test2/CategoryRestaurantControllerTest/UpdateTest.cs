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
    public class UpdateTest
    {
        private Mock<ICategoryRestaurantService> _mockCategoryRestaurantService;
        private CategoryRestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRestaurantService = new Mock<ICategoryRestaurantService>();
            _controller = new CategoryRestaurantController(_mockCategoryRestaurantService.Object);
        }

        // Kiểm tra trường hợp cập nhật với id hợp lệ và tên danh mục hợp lệ
        [TestMethod]
        public async Task UpdateCategoryRestaurant_WithValidData_ReturnsOk()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = 1,
                CategoryRestaurantName = "Italian Cuisine"
            };

            _mockCategoryRestaurantService.Setup(service => service.UpdateAsync(categoryRestaurantDto))
                .ReturnsAsync(true); // Giả lập cập nhật thành công

            // Act
            var result = await _controller.Update(categoryRestaurantDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
        }

        // Kiểm tra trường hợp cập nhật với id không hợp lệ (id = -1)
        [TestMethod]
        public async Task UpdateCategoryRestaurant_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = -1, // Id không hợp lệ
                CategoryRestaurantName = "Italian Cuisine"
            };

            _mockCategoryRestaurantService.Setup(service => service.UpdateAsync(categoryRestaurantDto))
                .ReturnsAsync(false); // Giả lập không tìm thấy danh mục

            // Act
            var result = await _controller.Update(categoryRestaurantDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.                       Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            var response = result.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
        }

        // Kiểm tra trường hợp cập nhật với CategoryRestaurantName = null
        [TestMethod]
        public async Task UpdateCategoryRestaurant_WithNullCategoryRestaurantName_ReturnsBadRequest()
        {
            // Arrange
            var categoryRestaurantDto = new CategoryRestaurantDto
            {
                CategoryRestaurantId = 1,
                CategoryRestaurantName = null // Tên danh mục là null
            };

            // Kiểm tra ModelState không hợp lệ khi CategoryRestaurantName null
            _controller.ModelState.AddModelError("CategoryRestaurantName", "Category Restaurant Name is required.");

            // Act
            var result = await _controller.Update(categoryRestaurantDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode); // Kiểm tra mã trạng thái BadRequest
            var response = result.Value as dynamic;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
        }

        }
    }
}
