using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RolesControllerTest
{
    [TestClass]
    public class UpdateRoleTest
    {
        private Mock<IRoleService> _mockRoleService;
        private RolesController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RolesController(_mockRoleService.Object);
        }

        [TestMethod]
        public async Task UpdateRole_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 1,
                Name = "" // Invalid Name (empty)
            };

            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.UpdateRole(roleDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }

        [TestMethod]
        public async Task UpdateRole_RoleUpdated_ReturnsNoContent()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 1,
                Name = "Admin"
            };

            _mockRoleService.Setup(s => s.UpdateAsync(roleDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateRole(roleDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(noContentResult);
        }

        [TestMethod]
        public async Task UpdateRole_RoleNotFound_ReturnsNotFound()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 999, // Non-existing RoleId
                Name = "Admin"
            };

            _mockRoleService.Setup(s => s.UpdateAsync(roleDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateRole(roleDto);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
        }
    }
}
