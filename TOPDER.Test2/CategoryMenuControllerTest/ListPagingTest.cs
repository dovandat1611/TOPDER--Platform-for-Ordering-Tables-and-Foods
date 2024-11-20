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
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TOPDER.Test2.CategoryMenuControllerTest
{
    [TestClass]
    public class ListPagingTest
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
        public async Task ListPaging_ValidRequest_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? categoryMenuName = "Menu";

            var mockPagedResult = new PaginatedList<CategoryMenuDto>(
                items: new List<CategoryMenuDto> { new CategoryMenuDto { CategoryMenuId = 1, CategoryMenuName = "Sample Menu" } },
                count: 1,
                pageIndex: 1,
                pageSize: 10);

            _mockCategoryMenuService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuName))
                .ReturnsAsync(mockPagedResult);

            // Act
            var result = await _controller.ListPaging(restaurantId, pageNumber, pageSize, categoryMenuName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<CategoryMenuDto>;
            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Items.Count);
            Assert.AreEqual("Sample Menu", response.Items.First().CategoryMenuName);
        }

        [TestMethod]
        public async Task ListPaging_InvalidRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = -1;
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = await _controller.ListPaging(restaurantId, pageNumber, pageSize);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Restaurant ID must be greater than zero.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ListPaging_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? categoryMenuName = null;

            _mockCategoryMenuService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuName))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.ListPaging(restaurantId, pageNumber, pageSize, categoryMenuName);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Đã xảy ra lỗi trong quá trình xử lý: Database connection failed", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task ListPaging_EmptyResult_ReturnsOkWithEmptyList()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? categoryMenuName = "NonExistingMenu";

            var mockPagedResult = new PaginatedList<CategoryMenuDto>(
                items: new List<CategoryMenuDto>(),
                count: 0,
                pageIndex: 1,
                pageSize: 10);

            _mockCategoryMenuService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuName))
                .ReturnsAsync(mockPagedResult);

            // Act
            var result = await _controller.ListPaging(restaurantId, pageNumber, pageSize, categoryMenuName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<CategoryMenuDto>;
            Assert.IsNotNull(response);
            Assert.AreEqual(0, response.Items.Count);
        }
    }
}
