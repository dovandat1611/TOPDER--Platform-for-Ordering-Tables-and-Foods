using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class GetCustomerMenuListTest
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
        public async Task GetCustomerMenuList_ValidRestaurantId_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;  // Valid restaurant ID
            var menuList = new List<MenuCustomerByCategoryMenuDto>
            {
                new MenuCustomerByCategoryMenuDto
                {
                    CategoryMenuId = 1,
                    CategoryMenuName = "Category 1",
                    MenusOfCategoryMenu = new List<MenuCustomerDto>
                    {
                        new MenuCustomerDto { MenuId = 101, DishName = "Dish 1", Price = 10.5m,  Image = "image1.jpg", Description = "Description 1" },
                        new MenuCustomerDto { MenuId = 102, DishName = "Dish 2", Price = 15.0m,  Image = "image2.jpg", Description = "Description 2" }
                    }
                }
            };

            _menuServiceMock.Setup(service => service.ListMenuCustomerByCategoryMenuAsync(restaurantId))
                .ReturnsAsync(menuList);
            // Act
            var result = await _controller.GetCustomerMenuList(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetCustomerMenuList_RestaurantIdNotFound_ReturnsOkWithEmptyList()
        {
            // Arrange
            int restaurantId = 9999;  // Non-existent restaurant ID

            _menuServiceMock.Setup(service => service.ListMenuCustomerByCategoryMenuAsync(restaurantId))
                .ReturnsAsync(new List<MenuCustomerByCategoryMenuDto>());  // Simulate empty list

            // Act
            var result = await _controller.GetCustomerMenuList(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
