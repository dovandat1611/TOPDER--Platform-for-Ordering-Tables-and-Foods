using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryRoomControllerTest
{
    [TestClass]
    public class UpdateCategoryRoomTest
    {
        private Mock<ICategoryRoomService> _mockCategoryRoomService;
        private CategoryRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRoomService = new Mock<ICategoryRoomService>();
            _controller = new CategoryRoomController(_mockCategoryRoomService.Object);

            // This simulates ModelState.IsValid being true when CategoryName is not null
            _controller.ModelState.Clear();
        }

        [TestMethod]
        public async Task UpdateCategoryRoom_WithValidIdAndCategoryName_ReturnsOk()
        {
            // Arrange
            var categoryRoomDto = new CategoryRoomDto
            {
                CategoryRoomId = 1,
                CategoryName = "Valid Room Name"
            };

            _mockCategoryRoomService.Setup(service => service.UpdateAsync(categoryRoomDto))
                .ReturnsAsync(true); // Simulate successful update

            // Act
            var result = await _controller.UpdateCategoryRoom(categoryRoomDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật Category Room với ID {categoryRoomDto.CategoryRoomId} thành công.", result.Value);
        }

        [TestMethod]
        public async Task UpdateCategoryRoom_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var categoryRoomDto = new CategoryRoomDto
            {
                CategoryRoomId = -1,
                CategoryName = "Non-existent Room"
            };

            _mockCategoryRoomService.Setup(service => service.UpdateAsync(categoryRoomDto))
                .ReturnsAsync(false); // Simulate ID not found

            // Act
            var result = await _controller.UpdateCategoryRoom(categoryRoomDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Room với ID {categoryRoomDto.CategoryRoomId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task UpdateCategoryRoom_WithNullCategoryName_ReturnsBadRequest()
        {
            // Arrange
            var categoryRoomDto = new CategoryRoomDto
            {
                CategoryRoomId = 1,
                CategoryName = null // Invalid name
            };

            // Simulate ModelState being invalid
            _controller.ModelState.AddModelError("CategoryName", "The CategoryName field is required.");

            // Act
            var result = await _controller.UpdateCategoryRoom(categoryRoomDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(result.Value is SerializableError); // Check that ModelState errors are returned
        }
    }
}
