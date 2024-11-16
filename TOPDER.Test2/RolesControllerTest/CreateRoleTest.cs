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
    public class CreateRoleTest
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
        public async Task CreateRole_ValidRoleDto_ReturnsOk()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 0,
                Name = "Admin"
            };

            _mockRoleService.Setup(s => s.AddAsync(roleDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateRole(roleDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Role thành công", okResult.Value);
        }

        [TestMethod]
        public async Task CreateRole_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 0,
                Name = "" // Invalid because Name is required
            };

            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.CreateRole(roleDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(((SerializableError)badRequestResult.Value).ContainsKey("Name"));
        }

        [TestMethod]
        public async Task CreateRole_ServiceFailure_ReturnsInternalServerError()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                RoleId = 0,
                Name = "Admin"
            };

            _mockRoleService.Setup(s => s.AddAsync(roleDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateRole(roleDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Error creating new role.", statusCodeResult.Value);
        }

    }
}
