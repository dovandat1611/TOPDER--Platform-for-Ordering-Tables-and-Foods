using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ChatControllerTest
{
    [TestClass]
    public class UpdateChatTests
    {
        private Mock<IChatService> _mockChatService;
        private ChatController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockChatService = new Mock<IChatService>();
            _controller = new ChatController(_mockChatService.Object);
        }

        [TestMethod]
        public async Task UpdateChat_WithValidDto_ReturnsOk()
        {
            // Arrange
            var updateChatDto = new UpdateChatDto { ChatId = 1, Content = "Updated message content" };
            _mockChatService.Setup(service => service.UpdateAsync(updateChatDto))
                .ReturnsAsync(true); // Simulate successful update

            // Act
            var result = await _controller.UpdateChat(updateChatDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật chat thành công.", result.Value);
        }

        [TestMethod]
        public async Task UpdateChat_WithNonexistentChat_ReturnsNotFound()
        {
            // Arrange
            var updateChatDto = new UpdateChatDto { ChatId = -1, Content = "Updated message content" };
            _mockChatService.Setup(service => service.UpdateAsync(updateChatDto))
                .ReturnsAsync(false); // Simulate chat not found

            // Act
            var result = await _controller.UpdateChat(updateChatDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat với ID {updateChatDto.ChatId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task UpdateChat_WithInvalidModel_ReturnsOk()
        {
            // Arrange
            var updateChatDto = new UpdateChatDto { ChatId = 1, Content = "" }; // Assume empty content is invalid
            _mockChatService.Setup(service => service.UpdateAsync(updateChatDto))
                .ReturnsAsync(true); // Simulate successful update

            // Act
            var result = await _controller.UpdateChat(updateChatDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật chat thành công.", result.Value);
        }
    }
}
