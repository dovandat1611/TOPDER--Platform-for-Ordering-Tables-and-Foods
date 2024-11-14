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
        public async Task GetMenuList_ValidRequest_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            var menuItems = new List<MenuRestaurantDto> { new MenuRestaurantDto { MenuId = 1, DishName = "Dish1" } };
            var paginatedList = new PaginatedList<MenuRestaurantDto>(menuItems, menuItems.Count, pageNumber, pageSize);

            _menuServiceMock.Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<MenuRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
        }

        [TestMethod]
        public async Task GetMenuList_RestaurantIdNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 999999;  // Non-existent restaurant ID
            int pageNumber = 1;
            int pageSize = 10;

            // Giả lập không tìm thấy món ăn nào cho nhà hàng có `restaurantId = 999999`
            _menuServiceMock.Setup(service => service.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, null, null))
                .ReturnsAsync(new PaginatedList<MenuRestaurantDto>(new List<MenuRestaurantDto>(), 0, pageNumber, pageSize));

            // Act
            var result = await _controller.GetMenuList(restaurantId, pageNumber, pageSize);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy món ăn nào cho nhà hàng được chỉ định.", notFoundResult.Value);
        }
    }
}
