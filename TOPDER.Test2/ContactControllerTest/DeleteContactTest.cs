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

namespace TOPDER.Test2.ContactControllerTest
{
    [TestClass]
    public class DeleteContactTest
    {
        private Mock<IContactService> _mockContactService;
        private Mock<ISendMailService> _mockSendMailService;
        private ContactController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockContactService = new Mock<IContactService>();
            _mockSendMailService = new Mock<ISendMailService>();
            _controller = new ContactController(_mockContactService.Object, _mockSendMailService.Object);
        }

        // Test case for valid ID (id = 1)
        [TestMethod]
        public async Task DeleteContact_WithValidId_ReturnsOk()
        {
            // Arrange
            var id = 1; // Valid ID
            _mockContactService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(true); // Simulate successful deletion

            // Act
            var result = await _controller.DeleteContact(id) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Xóa liên hệ với ID {id} thành công.", result.Value); // Assert success message
        }

        // Test case for invalid ID (id = -1)
        [TestMethod]
        public async Task DeleteContact_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = -1; // Invalid ID
            _mockContactService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(false); // Simulate failed deletion

            // Act
            var result = await _controller.DeleteContact(id) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Liên hệ với ID {id} không tồn tại.", result.Value); // Assert failure message
        }

        // Test case for non-existent ID (id = 999999)
        [TestMethod]
        public async Task DeleteContact_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var id = 999999; // Non-existent ID
            _mockContactService.Setup(service => service.RemoveAsync(id)).ReturnsAsync(false); // Simulate failed deletion

            // Act
            var result = await _controller.DeleteContact(id) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Liên hệ với ID {id} không tồn tại.", result.Value); // Assert failure message
        }
    }
}

