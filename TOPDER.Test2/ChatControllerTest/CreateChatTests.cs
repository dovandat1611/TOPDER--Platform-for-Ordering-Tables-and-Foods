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
    public class CreateChatTests
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
        public async Task CreateChat_WithValidData_ReturnsOk()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 1,
                ChatBy = 1,
                Content = "Hello!"
            };
            ChatDto chat = new ChatDto
            {
                ChatId = 1,
                ChatBoxId = 101,
                ChatTime = DateTime.Now, // Current time for the chat
                Content = "Hello!",
                ChatBy = 1,
                ChatByName = "John Doe",
            };
            _mockChatService.Setup(service => service.AddAsync(createChatDto)).ReturnsAsync(chat);

            // Act
            var result = await _controller.CreateChat(createChatDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo chat thành công.", result.Value);
        }

        

        [TestMethod]
        public async Task CreateChat_WithNonExistingChatBoxId_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 9999, // Non-existing ChatBoxId
                ChatBy = 1,
                Content = "Hello!"
            };


            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo chat thất bại.", result.Value);
        }

        [TestMethod]
        public async Task CreateChat_WithNegativeChatBoxId_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 1, // Invalid ChatBoxId
                ChatBy = 1,
                Content = ""
            };

            // Mark the model state as invalid for Content
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);

            // Cast result.Value to SerializableError and check the specific error message
            var errors = result.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(errors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(errors.ContainsKey("Content"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Content field is required.", ((string[])errors["Content"])[0]);
        }

        [TestMethod]
        public async Task CreateChat_WithNonExistingChatById_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 1,
                ChatBy = 99999, // Non-existing ChatBy ID
                Content = "Hello!"
            };


            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo chat thất bại.", result.Value);
        }

        [TestMethod]
        public async Task CreateChat_WithNegativeChatBoxIdAndChatByAndNullContent_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = -1,
                ChatBy = -1, // Non-existing ChatBy ID
                Content = "Hello!"   // Null Content
            };

            // Mark the model state as invalid for Content
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);

            // Cast result.Value to SerializableError and check the specific error message
            var errors = result.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(errors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(errors.ContainsKey("Content"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Content field is required.", ((string[])errors["Content"])[0]);
        }
    }
}
