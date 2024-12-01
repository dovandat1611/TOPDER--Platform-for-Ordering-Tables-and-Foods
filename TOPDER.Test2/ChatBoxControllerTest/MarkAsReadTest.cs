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
    public class MarkAsReadTest
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
        public async Task MarkAsRead_ReturnsOk_WhenIsReadAsyncReturnsTrue()
        {
            // Arrange
            int uid = 1;
            int chatboxId = 1;
            _mockChatBoxService
                .Setup(service => service.IsReadAsync(uid, chatboxId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkAsRead(uid, chatboxId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task MarkAsRead_ReturnsNotFound_WhenIsReadAsyncReturnsFalse()
        {
            // Arrange
            int uid = 1;
            int chatboxId = -1;
            _mockChatBoxService
                .Setup(service => service.IsReadAsync(uid, chatboxId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkAsRead(uid, chatboxId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
