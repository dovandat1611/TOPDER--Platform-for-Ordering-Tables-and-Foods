using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.RolesControllerTest
{
    [TestClass]
    public class GetRolesTest
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
        public async Task GetRoles_ValidParameters_ReturnsOkWithPaginatedResponse()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            var roles = new PaginatedList<RoleDto>(
                new List<RoleDto>
                {
                new RoleDto { RoleId = 1, Name = "Admin" },
                new RoleDto { RoleId = 2, Name = "User" }
                },
                 2,
                pageIndex: pageNumber,
                pageSize: pageSize
            );

            _mockRoleService.Setup(s => s.GetPagingAsync(pageNumber, pageSize))
                            .ReturnsAsync(roles);

            // Act
            var result = await _controller.GetRoles(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);

            var response = okResult.Value as PaginatedResponseDto<RoleDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasPreviousPage);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasNextPage);
        }

        [TestMethod]
        public async Task GetRoles_EmptyResult_ReturnsOkWithEmptyResponse()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            var roles = new PaginatedList<RoleDto>(
                new List<RoleDto>(), // Empty list
             0,
                pageIndex: pageNumber,
                pageSize: pageSize
            );

            _mockRoleService.Setup(s => s.GetPagingAsync(pageNumber, pageSize))
                            .ReturnsAsync(roles);

            // Act
            var result = await _controller.GetRoles(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);

            var response = okResult.Value as PaginatedResponseDto<RoleDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.TotalPages);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasPreviousPage);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(response.HasNextPage);
        }
    }
}
