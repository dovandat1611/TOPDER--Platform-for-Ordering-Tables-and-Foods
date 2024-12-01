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
    public class MarkAllAsReadTest
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
        public async Task MarkAllAsRead_ReturnsOk_WhenIsReadAllAsyncReturnsTrue()
        {
            // Arrange
            int uid = 1;
            _mockChatBoxService
                .Setup(service => service.IsReadAllAsync(uid))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkAllAsRead(uid);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task MarkAllAsRead_ReturnsNotFound_WhenIsReadAllAsyncReturnsFalse()
        {
            // Arrange
            int uid = -1;
            _mockChatBoxService
                .Setup(service => service.IsReadAllAsync(uid))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkAllAsRead(uid);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
