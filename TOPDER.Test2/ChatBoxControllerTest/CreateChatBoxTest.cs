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
    public class CreateChatBoxTest
    {
        private Mock<IChatBoxService> _mockChatBoxService;
        private ChatBoxController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _mockChatBoxService = new Mock<IChatBoxService>();
            _controller = new ChatBoxController(_mockChatBoxService.Object);
        }

        [TestMethod]
        public async Task CreateChatBox_WithValidData_ReturnsOk()
        {
            // Arrange
            var chatBoxDto = new CreateChatBoxDto
            {
                ChatBoxId = 1,
                CustomerId = 1,
                RestaurantId = 1
            };

            _mockChatBoxService.Setup(service => service.AddAsync(chatBoxDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateChatBox(chatBoxDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Chat Box thành công.", result.Value);
        }

        [TestMethod]
        public async Task CreateChatBox_WithNonExistentCustomerId_ReturnsBadRequest()
        {
            // Arrange
            var chatBoxDto = new CreateChatBoxDto
            {
                ChatBoxId = 2,
                CustomerId = 99999, // Non-existent customer
                RestaurantId = 1
            };

            _mockChatBoxService.Setup(service => service.AddAsync(chatBoxDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateChatBox(chatBoxDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Chat Box thất bại.", result.Value);
        }

        [TestMethod]
        public async Task CreateChatBox_WithNegativeCustomerId_ReturnsBadRequest()
        {
            // Arrange
            var chatBoxDto = new CreateChatBoxDto
            {
                ChatBoxId = 3,
                CustomerId = -1, // Invalid negative ID
                RestaurantId = 1
            };

            _mockChatBoxService.Setup(service => service.AddAsync(chatBoxDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateChatBox(chatBoxDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Chat Box thất bại.", result.Value);
        }

        [TestMethod]
        public async Task CreateChatBox_WithNonExistentRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var chatBoxDto = new CreateChatBoxDto
            {
                ChatBoxId = 4,
                CustomerId = 1,
                RestaurantId = 9999 // Non-existent restaurant
            };

            _mockChatBoxService.Setup(service => service.AddAsync(chatBoxDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateChatBox(chatBoxDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Chat Box thất bại.", result.Value);
        }

        [TestMethod]
        public async Task CreateChatBox_WithNegativeRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var chatBoxDto = new CreateChatBoxDto
            {
                ChatBoxId = 5,
                CustomerId = 1,
                RestaurantId = -1 // Invalid negative ID
            };

            _mockChatBoxService.Setup(service => service.AddAsync(chatBoxDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateChatBox(chatBoxDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Chat Box thất bại.", result.Value);
        }
    }

}
