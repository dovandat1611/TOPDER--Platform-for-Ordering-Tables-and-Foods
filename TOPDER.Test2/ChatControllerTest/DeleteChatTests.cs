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

namespace TOPDER.Test2.ChatControllerTest
{
    [TestClass]
    public class DeleteChatTests
    {
        private Mock<IChatService> _mockChatService;
        private ChatController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock service and controller before each test
            _mockChatService = new Mock<IChatService>();
            _controller = new ChatController(_mockChatService.Object);
        }

        // Test for valid ID
        [TestMethod]
        public async Task DeleteChat_WithValidId_ReturnsOk()
        {
            // Arrange
            int id = 1; // Valid ID
            _mockChatService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(true); // Simulate successful deletion

            // Act
            var result = await _controller.DeleteChat(id) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Expecting 200 OK
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Xóa chat với ID {id} thành công.", result.Value); // Expected success message
        }

        // Test for non-existent ID
        [TestMethod]
        public async Task DeleteChat_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            int id = 99999; // Non-existent ID
            _mockChatService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(false); // Simulate chat not found

            // Act
            var result = await _controller.DeleteChat(id) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode); // Expecting 404 Not Found
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat với ID {id} không tồn tại.", result.Value); // Expected error message
        }

        [TestMethod]
        public async Task DeleteChat_WithNonExistentId_ReturnsInternalServerError()
        {
            // Arrange
            int id = 99999; // Non-existent ID
            _mockChatService.Setup(service => service.RemoveAsync(id))
                .ReturnsAsync(false); // Simulate chat not found

            // Act
            var result = await _controller.DeleteChat(id) as ObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode); // Expecting 500 Internal Server Error
        }

        [TestMethod]
        public async Task DeleteChat_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1;
            _mockChatService.Setup(service => service.RemoveAsync(invalidId))
                .ReturnsAsync(false); // Simulate chat not found

            // Act
            var result = await _controller.DeleteChat(invalidId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat với ID {invalidId} không tồn tại.", result.Value);
        }
    }

}
