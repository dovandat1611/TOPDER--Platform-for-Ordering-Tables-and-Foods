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
        public async Task ListPaging_WithRestaurantIdZero_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = 0; // Invalid restaurantId

            // Act
            var result = await _controller.ListPaging(restaurantId) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Restaurant ID must be greater than zero.", result.Value);
        }

        [TestMethod]
        public async Task ListPaging_WithRestaurantIdNegative_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId

            // Act
            var result = await _controller.ListPaging(restaurantId) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Restaurant ID must be greater than zero.", result.Value);
        }

        //[TestMethod]
        //public async Task ListPaging_WithValidRestaurantIdAndNullCategoryMenuName_ReturnsStatus200()
        //{
        //    int restaurantId = 3;
        //    int pageNumber = 1;
        //    int pageSize = 10;
        //    string? categoryMenuName = null;

        //    // Act
        //    var result = await _controller.ListPaging(restaurantId, pageNumber, pageSize, categoryMenuName) as OkObjectResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(200, result.StatusCode);
        //}
    }
}
