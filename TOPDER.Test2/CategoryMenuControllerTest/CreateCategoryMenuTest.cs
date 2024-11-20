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
    public class CreateCategoryMenuTest
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
        public async Task CreateCategoryMenu_WithValidCategoryMenuName_ReturnsOk()
        {
            // Arrange
            var categoryMenuDto = new CreateCategoryMenuDto
            {
                RestaurantId = 1,
                CategoryMenuName = "Appetizers"
            };
            _mockCategoryMenuService.Setup(service => service.AddAsync(categoryMenuDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateCategoryMenu(categoryMenuDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Category Menu thành công.", result.Value);
        }

        [TestMethod]
        public async Task CreateCategoryMenu_WithNullCategoryMenuName_ReturnsBadRequest()
        {
            // Arrange
            var categoryMenuDto = new CreateCategoryMenuDto
            {
                RestaurantId = 1,
                CategoryMenuName = null
            };
            _controller.ModelState.AddModelError("CategoryMenuName", "The CategoryMenuName field is required.");

            // Act
            var result = await _controller.CreateCategoryMenu(categoryMenuDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError));
        }

        [TestMethod]
        public async Task CreateCategoryMenu_WithServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            var categoryMenuDto = new CreateCategoryMenuDto
            {
                RestaurantId = -1,
                CategoryMenuName = "Appetizers"
            };
            _mockCategoryMenuService.Setup(service => service.AddAsync(categoryMenuDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateCategoryMenu(categoryMenuDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Category Menu thất bại.", result.Value);
        }
    }
}
