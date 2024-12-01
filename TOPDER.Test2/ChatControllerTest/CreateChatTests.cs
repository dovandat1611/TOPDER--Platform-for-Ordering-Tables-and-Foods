using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

            // Mock the IHubContext<AppHub> and IHubClients
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();
            var mockClients = new Mock<IHubClients>(); // Mock the IHubClients
            var mockClientProxy = new Mock<IClientProxy>(); // Mock the IClientProxy (to use with SendAsync)

            // Set up the IHubClients.All to return the IClientProxy mock
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            // Set up IHubContext to return the mock IHubClients
            _mockSignalRHub.Setup(hub => hub.Clients).Returns(mockClients.Object);

            // Creating the controller instance with the mocked dependencies
            _controller = new ChatController(_mockChatService.Object, _mockSignalRHub.Object);
        }

        [TestMethod]
        public async Task CreateChat_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 1,
                ChatBy = 1,
                Content = "" // Invalid content
            };

            // Mark the model state as invalid for Content
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);

            var errors = result.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(errors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(errors.ContainsKey("Content"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Content field is required.", ((string[])errors["Content"])[0]);
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

            var chat = new ChatDto
            {
                ChatId = 1,
                ChatBoxId = 1,
                ChatTime = DateTime.Now,
                Content = "Hello!",
                ChatBy = 1,
                ChatByName = "John Doe"
            };

            // Mock the AddAsync method to return a chat DTO
            _mockChatService.Setup(service => service.AddAsync(createChatDto)).ReturnsAsync(chat);

            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _mockSignalRHub.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);

            // Act
            var result = await _controller.CreateChat(createChatDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(chat, result.Value);

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

            _mockChatService.Setup(service => service.AddAsync(createChatDto)).ReturnsAsync((ChatDto)null);

            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo chat thất bại.", result.Value);
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

            _mockChatService.Setup(service => service.AddAsync(createChatDto)).ReturnsAsync((ChatDto)null);

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
                ChatBoxId = -1, // Invalid ChatBoxId
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
        public async Task CreateChat_WithNullContent_ReturnsBadRequest()
        {
            // Arrange
            var createChatDto = new CreateChatDto
            {
                ChatBoxId = 1,
                ChatBy = 1,
                Content = null // Null Content
            };

            // Mark the model state as invalid for Content
            _controller.ModelState.AddModelError("Content", "The Content field is required.");

            // Act
            var result = await _controller.CreateChat(createChatDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);

            var errors = result.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(errors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(errors.ContainsKey("Content"));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Content field is required.", ((string[])errors["Content"])[0]);
        }
    }
}
