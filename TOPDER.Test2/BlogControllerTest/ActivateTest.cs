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
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.BlogControllerTest
{
    [TestClass]
    public class ActivateTest
    {
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private Mock<IBlogService> _mockBlogService;
        private BlogController _controller;

        [TestInitialize]
        public void Initialize()
        {
            // Mock dependencies
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _mockBlogService = new Mock<IBlogService>();
            // Create instance of the controller with mocked dependencies
            _controller = new BlogController(_mockBlogService.Object, _mockCloudinaryService.Object);
        }
        [TestMethod]
        public async Task Activate_ValidStatus_ReturnsOk()
        {
            // Arrange
            var blogId = 1;
            var status = Common_Status.ACTIVE; // Assume Common_Status.ACTIVE is a valid status
            _mockBlogService.Setup(service => service.UpdateStatusAsync(blogId, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Activate(blogId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Blog đã được cập nhật.", okResult.Value);
        }

        [TestMethod]
        public async Task Activate_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var blogId = 1;
            var status = "InvalidStatus"; // Invalid status
            var expectedMessage = $"Status phải giống {Common_Status.INACTIVE} hoặc {Common_Status.ACTIVE}.";

            // Act
            var result = await _controller.Activate(blogId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedMessage, badRequestResult.Value);
        }

        [TestMethod]
        public async Task Activate_BlogNotFound_ReturnsNotFound()
        {
            // Arrange
            var blogId = -1;
            var status = Common_Status.ACTIVE; // Valid status
            _mockBlogService.Setup(service => service.UpdateStatusAsync(blogId, status))
                .ReturnsAsync(false); // Simulate not found or update failure

            // Act
            var result = await _controller.Activate(blogId, status);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy Blog hoặc update lỗi!.", notFoundResult.Value);
        }
    }
}
