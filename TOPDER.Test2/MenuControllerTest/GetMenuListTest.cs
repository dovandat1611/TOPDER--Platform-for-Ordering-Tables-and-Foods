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
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class GetMenuListTest
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
        public async Task GetMenuList_NoMenuFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            int? categoryMenuId = null;
            string? menuName = null;

            _menuServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName))
                .ReturnsAsync((PaginatedList<MenuRestaurantDto>)null); // Không có món ăn nào

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize, categoryMenuId, menuName);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy món ăn nào cho nhà hàng được chỉ định.", notFoundResult?.Value);
        }

        [TestMethod]
        public async Task GetMenuList_MenuFound_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            int? categoryMenuId = null;
            string? menuName = null;

            var menuList = new PaginatedList<MenuRestaurantDto>(new List<MenuRestaurantDto>
            {
                new MenuRestaurantDto
                {
                    MenuId = 1,
                    CategoryMenuId = 1,
                    CategoryMenuName = "Appetizers",
                    DishName = "Spring Rolls",
                    Price = 50,
                    Image = "springrolls.jpg",
                    Description = "Crispy and delicious",
                    Status = "Available"
                },
                new MenuRestaurantDto
                {
                    MenuId = 2,
                    CategoryMenuId = 1,
                    CategoryMenuName = "Appetizers",
                    DishName = "Salad",
                    Price = 30,
                    Image = "salad.jpg",
                    Description = "Fresh and healthy",
                    Status = "Available"
                }
            }, 1, 1,10);

            _menuServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName))
                .ReturnsAsync(menuList);

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize, categoryMenuId, menuName);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);

            var response = okResult?.Value as PaginatedResponseDto<MenuRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
        }
        [TestMethod]
        public async Task GetMenuList_InvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 99999; // Không tồn tại
            int pageNumber = 1;
            int pageSize = 10;
            int? categoryMenuId = null;
            string? menuName = null;

            _menuServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName))
                .ReturnsAsync((PaginatedList<MenuRestaurantDto>)null);

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize, categoryMenuId, menuName);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy món ăn nào cho nhà hàng được chỉ định.", notFoundResult?.Value);
        }
        [TestMethod]
        public async Task GetMenuList_CategoryMenuIdAndMenuNameProvided_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            int? categoryMenuId = 1; // Specific category menu ID
            string? menuName = "Spring Rolls"; // Specific menu name

            var menuList = new PaginatedList<MenuRestaurantDto>(new List<MenuRestaurantDto>
            {
                new MenuRestaurantDto
                {
                    MenuId = 1,
                    CategoryMenuId = 1,
                    CategoryMenuName = "Appetizers",
                    DishName = "Spring Rolls",
                    Price = 50,
                    Image = "springrolls.jpg",
                    Description = "Crispy and delicious",
                    Status = "Available"
                },
                new MenuRestaurantDto
                {
                    MenuId = 2,
                    CategoryMenuId = 1,
                    CategoryMenuName = "Appetizers",
                    DishName = "Salad",
                    Price = 30,
                    Image = "salad.jpg",
                    Description = "Fresh and healthy",
                    Status = "Available"
                }
            }, 1, 1, 10);

            _menuServiceMock
                .Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName))
                .ReturnsAsync(menuList);

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize, categoryMenuId, menuName);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);

        }

    }
}
