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

namespace TOPDER.Test2.ChatBoxControllerTest
{
    [TestClass]
    public class DeleteChatBoxTests
    {
        private Mock<IChatBoxService> _mockChatBoxService;
        private ChatBoxController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockChatBoxService = new Mock<IChatBoxService>();
            _controller = new ChatBoxController(_mockChatBoxService.Object);
        }

        [TestMethod]
        public async Task DeleteChatBox_WithValidId_ReturnsOk()
        {
            // Arrange
            int chatBoxId = 1; // Valid ID
            _mockChatBoxService.Setup(service => service.RemoveAsync(chatBoxId))
                .ReturnsAsync(true); // Simulate successful deletion

            // Act
            var result = await _controller.DeleteChatBox(chatBoxId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Xóa Chat Box với ID {chatBoxId} thành công.", result.Value);
        }

        [TestMethod]
        public async Task DeleteChatBox_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            int chatBoxId = 999; // Non-existent ID
            _mockChatBoxService.Setup(service => service.RemoveAsync(chatBoxId))
                .ReturnsAsync(false); // Simulate chat box not found

            // Act
            var result = await _controller.DeleteChatBox(chatBoxId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat Box với ID {chatBoxId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task DeleteChatBox_WithNegativeId_ReturnsNotFound()
        {
            // Arrange
            int chatBoxId = -1; // Invalid (negative) ID
            _mockChatBoxService.Setup(service => service.RemoveAsync(chatBoxId))
                .ReturnsAsync(false); // Simulate chat box not found

            // Act
            var result = await _controller.DeleteChatBox(chatBoxId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat Box với ID {chatBoxId} không tồn tại.", result.Value);
        }

        
    }

}
