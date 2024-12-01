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
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ChatControllerTest
{
    [TestClass]
    public class GetChatListTests
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
        public async Task GetChatList_WithValidId_ReturnsOk()
        {
            // Arrange
            int chatBoxId = 1; // Valid chatBoxId
            var chatList = new List<ChatDto>
        {
            new ChatDto { ChatId = 1, ChatBoxId = chatBoxId, Content = "Message 1" },
            new ChatDto { ChatId = 2, ChatBoxId = chatBoxId, Content = "Message 2" }
        };

            _mockChatService.Setup(service => service.GetListAsync(chatBoxId))
                .ReturnsAsync(chatList); // Simulating valid chat list for the given chatBoxId

            // Act
            var result = await _controller.GetChatList(chatBoxId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Expecting 200 OK
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(chatList, result.Value); // The response should contain the chat list
        }

        [TestMethod]
        public async Task GetChatList_WithNegativeId_ReturnsOk()
        {
            // Arrange
            int chatBoxId = -1; // Valid chatBoxId
        
            // Act
            var result = await _controller.GetChatList(chatBoxId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Expecting 200 OK
        }

        [TestMethod]
        public async Task GetChatList_WithNonExistentId_ReturnsOk()
        {
            // Arrange
            int chatBoxId = 99999; // Valid chatBoxId

            // Act
            var result = await _controller.GetChatList(chatBoxId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Expecting 200 OK
        }
    }

}
