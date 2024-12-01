using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ChatControllerTest
{
    [TestClass]
    public class GetChatTests
    {
        private Mock<IChatService> _mockChatService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;
        private ChatController _controller;

        [TestInitialize]
        public void SetUp()
        {
            // Mocking the dependencies
            _mockChatService = new Mock<IChatService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();

            // Creating the controller instance with the mocked dependencies
            _controller = new ChatController(_mockChatService.Object, _mockSignalRHub.Object);
        }


        [TestMethod]
        public async Task GetChat_WithValidId_ReturnsOk()
        {
            // Arrange
            int chatId = 1; // Simulating a non-existent chatId


           

            // Act
            var result = await _controller.GetChat(chatId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }


        [TestMethod]
        public async Task GetChat_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int chatId = 9999; // Simulating a non-existent chatId
            _mockChatService.Setup(service => service.GetItemAsync(chatId))
                .ThrowsAsync(new KeyNotFoundException()); // Simulate the chat not found

            // Act
            var result = await _controller.GetChat(chatId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.                   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat với ID {chatId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task GetChat_WithNegativeId_ReturnsNotFound()
        {
            // Arrange
            int chatId = -1; // Invalid chatId (negative value)
            _mockChatService.Setup(service => service.GetItemAsync(chatId))
                .ThrowsAsync(new KeyNotFoundException()); // Simulate the chat not found

            // Act
            var result = await _controller.GetChat(chatId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Chat với ID {chatId} không tồn tại.", result.Value);
        }
    }
}
