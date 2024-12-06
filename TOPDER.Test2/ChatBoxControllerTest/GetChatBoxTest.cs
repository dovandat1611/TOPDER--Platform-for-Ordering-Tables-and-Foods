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
    public class GetChatBoxTest
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
        public async Task GetChatBox_ShouldReturnOk_WhenChatBoxExists()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = 1;

            var chatBoxDto = new ChatBoxDto
            {
                ChatBoxId = 100,
                CustomerId = customerId,
                RestaurantId = restaurantId
            };

            _mockChatBoxService
                .Setup(s => s.CheckExistAsync(customerId, restaurantId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.GetChatBox(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
        }

        [TestMethod]
        public async Task GetChatBox_ShouldReturnOk_WhenCustomerIdIsNegative()
        {
            // Arrange
            int customerId = -1;
            int restaurantId = 1;

            _mockChatBoxService
                .Setup(s => s.CheckExistAsync(customerId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetChatBox(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetChatBox_ShouldReturnOk_WhenRestaurantIdIsNegative()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = -1;

            _mockChatBoxService
                .Setup(s => s.CheckExistAsync(customerId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetChatBox(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

    }
}
