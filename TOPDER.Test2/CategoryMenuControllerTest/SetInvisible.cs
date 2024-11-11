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

namespace TOPDER.Test2.CategoryMenuControllerTest
{
    [TestClass]
    public class SetInvisible
    {
        private Mock<ICategoryMenuService> _mockCategoryMenuService;
        private CategoryMenuController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryMenuService = new Mock<ICategoryMenuService>();
            _controller = new CategoryMenuController(_mockCategoryMenuService.Object);
        }

        [TestMethod]
        public async Task SetInvisible_WithValidId_ReturnsOk()
        {
            // Arrange
            int id = 1; // Valid ID
            _mockCategoryMenuService.Setup(service => service.InvisibleAsync(id))
                .ReturnsAsync(true); // Simulate success

            // Act
            var result = await _controller.SetInvisible(id) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("CategoryMenu và các menu liên quan đã được ẩn thành công.", result.Value);
        }

        [TestMethod]
        public async Task SetInvisible_WithIdZero_ReturnsNotFound()
        {
            // Arrange
            int id = 0; // Invalid ID
            _mockCategoryMenuService.Setup(service => service.InvisibleAsync(id))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.SetInvisible(id) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy CategoryMenu với ID tương ứng.", result.Value);
        }

        [TestMethod]
        public async Task SetInvisible_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int id = -1; // Invalid ID
            _mockCategoryMenuService.Setup(service => service.InvisibleAsync(id))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.SetInvisible(id) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy CategoryMenu với ID tương ứng.", result.Value);
        }
    }
}
