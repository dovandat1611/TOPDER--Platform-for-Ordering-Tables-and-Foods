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

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class UpdateMenuStatusTest
    {
        private Mock<IMenuService> _menuServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private MenuController _controller;

        [TestInitialize]
        public void Setup()
        {
            _menuServiceMock = new Mock<IMenuService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new MenuController(_menuServiceMock.Object, _cloudinaryServiceMock.Object);
        }

        [TestMethod]
        public async Task UpdateMenuStatus_ValidStatus_ReturnsOk()
        {
            // Arrange
            var menuId = 1;
            var status = "ACTIVE";  // Valid status

            // Simulate successful update
            _menuServiceMock.Setup(service => service.IsActiveAsync(menuId, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateMenuStatus(menuId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateMenuStatus_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var menuId = 1;
            var status = "INVALID";  // Invalid status

            // Act
            var result = await _controller.UpdateMenuStatus(menuId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateMenuStatus_MenuNotFound_ReturnsBadRequest()
        {
            // Arrange
            var menuId = 99999;  // Simulate a non-existing menu ID
            var status = "ACTIVE";

            // Simulate failed update (e.g., menu not found)
            _menuServiceMock.Setup(service => service.IsActiveAsync(menuId, status))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateMenuStatus(menuId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

    }
}
