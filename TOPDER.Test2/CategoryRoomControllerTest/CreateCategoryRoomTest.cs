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
    public class CreateCategoryRoomTest
    {
        private Mock<ICategoryRoomService> _mockCategoryRoomService;
        private CategoryRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRoomService = new Mock<ICategoryRoomService>();
            _controller = new CategoryRoomController(_mockCategoryRoomService.Object);
        }

        [TestMethod]
        public async Task CreateCategoryRoom_WithValidData_ReturnsOk()
        {
            // Arrange
            var categoryRoomDto = new CategoryRoomDto
            {
                CategoryRoomId = 1,
                CategoryName = "Luxury Room"
            };

            _mockCategoryRoomService.Setup(service => service.AddAsync(categoryRoomDto))
                .ReturnsAsync(true); // Simulate success

            // Act
            var result = await _controller.CreateCategoryRoom(categoryRoomDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Category Room thành công.", result.Value);
        }

        [TestMethod]
        public async Task CreateCategoryRoom_WhenModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            var categoryRoomDto = new CategoryRoomDto
            {
                CategoryRoomId = 1,
                CategoryName = "" // Invalid CategoryName (empty string)
            };

            _controller.ModelState.AddModelError("CategoryName", "CategoryName is required");

            // Act
            var result = await _controller.CreateCategoryRoom(categoryRoomDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
        }

    }
}
