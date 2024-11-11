﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryRestaurantControllerTest
{
    [TestClass]
    public class GetAllCategoryRestaurantsTest
    {
        private Mock<ICategoryRestaurantService> _mockCategoryRestaurantService;
        private CategoryRestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRestaurantService = new Mock<ICategoryRestaurantService>();
            _controller = new CategoryRestaurantController(_mockCategoryRestaurantService.Object);
        }
        [TestMethod]
        public async Task GetAllCategoryRestaurants_ReturnsOk()
        {
            // Arrange
            var mockCategoryRestaurants = new List<CategoryRestaurantDto>
    {
        new CategoryRestaurantDto { CategoryRestaurantId = 1, CategoryRestaurantName = "Italian Cuisine" },
        new CategoryRestaurantDto { CategoryRestaurantId = 2, CategoryRestaurantName = "Chinese Cuisine" }
    };

            _mockCategoryRestaurantService.Setup(service => service.GetAllCategoryRestaurantAsync())
                .ReturnsAsync(mockCategoryRestaurants); // Giả lập trả về danh sách Category Restaurant

            // Act
            var result = await _controller.GetAllCategoryRestaurants() as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode); // Kiểm tra mã trạng thái HTTP
            var response = result.Value as List<CategoryRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response); // Kiểm tra dữ liệu trả về
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count); // Kiểm tra số lượng phần tử trả về
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Italian Cuisine", response[0].CategoryRestaurantName); // Kiểm tra tên Category Restaurant đầu tiên
        }
    }
}
