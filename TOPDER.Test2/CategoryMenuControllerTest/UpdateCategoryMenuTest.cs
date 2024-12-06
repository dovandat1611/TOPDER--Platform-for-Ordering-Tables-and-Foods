using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryMenuControllerTest
{
    [TestClass]
    public class UpdateCategoryMenuTest
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
        public async Task UpdateCategoryMenu_WithValidId_ReturnsOk()
        {
            // Arrange
            var categoryMenuDto = new CategoryMenuDto
            {
                CategoryMenuId = 1,
                CategoryMenuName = "Main Course"
            };

            _mockCategoryMenuService.Setup(service => service.UpdateAsync(categoryMenuDto))
                .ReturnsAsync(true); // Simulate success

            // Act
            var result = await _controller.UpdateCategoryMenu(categoryMenuDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật Category Menu với ID {categoryMenuDto.CategoryMenuId} thành công.", result.Value);
        }

        [TestMethod]
        public async Task UpdateCategoryMenu_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryMenuDto = new CategoryMenuDto
            {
                CategoryMenuId = -1,
                CategoryMenuName = "Main Course"
            };

            _mockCategoryMenuService.Setup(service => service.UpdateAsync(categoryMenuDto))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.UpdateCategoryMenu(categoryMenuDto) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Menu với ID {categoryMenuDto.CategoryMenuId} không tồn tại.", result.Value);
        }

        

        [TestMethod]
        public async Task UpdateCategoryMenu_WithNullCategoryMenuName_ReturnsOk()
        {
            // Arrange
            var categoryMenuDto = new CategoryMenuDto
            {
                CategoryMenuId = 1,
                CategoryMenuName = "" // CategoryMenuName is null
            };

            _mockCategoryMenuService.Setup(service => service.UpdateAsync(categoryMenuDto))
                .ReturnsAsync(true); // Simulate success

            // Act
            var result = await _controller.UpdateCategoryMenu(categoryMenuDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật Category Menu với ID {categoryMenuDto.CategoryMenuId} thành công.", result.Value);
        }
    }
}
