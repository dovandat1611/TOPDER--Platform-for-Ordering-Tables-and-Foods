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

namespace TOPDER.Test2.CategoryRoomControllerTest
{
    [TestClass]
    public class SetInvisibleTest
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
        public async Task SetInvisible_WithValidId_ReturnsOk()
        {
            // Arrange
            int categoryRoomId = 1; // Valid ID
            _mockCategoryRoomService.Setup(service => service.InvisibleAsync(categoryRoomId))
                .ReturnsAsync(true); // Simulate successful invisibility

            // Act
            var result = await _controller.SetInvisible(categoryRoomId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public async Task SetInvisible_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            int categoryRoomId = -1; // Invalid ID
                                     // Act
            var result = await _controller.SetInvisible(categoryRoomId) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task SetInvisible_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            int categoryRoomId = 999999; // Valid ID but non-existent
            _mockCategoryRoomService.Setup(service => service.InvisibleAsync(categoryRoomId))
                .ReturnsAsync(false); // Simulate not found

            // Act
            var result = await _controller.SetInvisible(categoryRoomId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task SetInvisible_WithException_ReturnsInternalServerError()
        {
            // Arrange
            int categoryRoomId = 1; // Valid ID
            _mockCategoryRoomService.Setup(service => service.InvisibleAsync(categoryRoomId))
                .ThrowsAsync(new Exception("Unexpected error")); // Simulate unexpected error

            // Act
            var result = await _controller.SetInvisible(categoryRoomId) as ObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(500, result.StatusCode);
        }
    }
}
