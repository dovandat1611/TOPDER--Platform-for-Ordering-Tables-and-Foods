using Microsoft.AspNetCore.Http;
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
    public class GetRoleByIdTest
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
        public async Task GetRoleById_ExistingRole_ReturnsOk()
        {
            // Arrange
            int roleId = 1;
            var roleDto = new RoleDto
            {
                RoleId = roleId,
                Name = "Admin"
            };

            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ReturnsAsync(roleDto);

            // Act
            var result = await _controller.GetRoleById(roleId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(roleDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetRoleById_RoleNotFound_ReturnsNotFound()
        {
            // Arrange
            int roleId = 999; // Non-existing role
            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ThrowsAsync(new KeyNotFoundException("Role not found"));

            // Act
            var result = await _controller.GetRoleById(roleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Role not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetRoleById_InternalError_ReturnsInternalServerError()
        {
            // Arrange
            int roleId = 1;
            _mockRoleService.Setup(s => s.GetByIdAsync(roleId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetRoleById(roleId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Error retrieving role: Unexpected error", statusCodeResult.Value);
        }
    }
}
