using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ChatBoxControllerTest
{
    [TestClass]
    public class GetChatBoxPagingTests
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
        public async Task GetChatBoxPaging_WithValidUserId_ReturnsOk()
        {
            // Arrange
            int userId = 1; // Valid user ID
            var chatBoxList = new List<ChatBoxDto> { new ChatBoxDto { ChatBoxId = 1, CustomerId = userId, RestaurantId = 10 } };

            _mockChatBoxService.Setup(service => service.GetChatListAsync(userId))
                .ReturnsAsync(chatBoxList);

            // Act
            var result = await _controller.GetChatBoxPaging(userId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.AreEqual(200, result.StatusCode);
            CollectionAssert.AreEquivalent(chatBoxList, result.Value as List<ChatBoxDto>);
        }

        [TestMethod]
        public async Task GetChatBoxPaging_WithInvalidUserId_ReturnsOkWithEmptyList()
        {
            // Arrange
            int userId = -1; // Invalid user ID
            var emptyChatBoxList = new List<ChatBoxDto>();

            _mockChatBoxService.Setup(service => service.GetChatListAsync(userId))
                .ReturnsAsync(emptyChatBoxList);

            // Act
            var result = await _controller.GetChatBoxPaging(userId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            CollectionAssert.AreEqual(emptyChatBoxList, result.Value as List<ChatBoxDto>);
        }

        [TestMethod]
        public async Task GetChatBoxPaging_WithNonExistentUserId_ReturnsOkWithEmptyList()
        {
            // Arrange
            int userId = 9999; // Non-existent user ID
            var emptyChatBoxList = new List<ChatBoxDto>();

            _mockChatBoxService.Setup(service => service.GetChatListAsync(userId))
                .ReturnsAsync(emptyChatBoxList);

            // Act
            var result = await _controller.GetChatBoxPaging(userId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            CollectionAssert.AreEqual(emptyChatBoxList, result.Value as List<ChatBoxDto>);
        }
    }

}
