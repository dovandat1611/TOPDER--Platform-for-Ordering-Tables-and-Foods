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
            List<CategoryRestaurantViewDto> categoryList = new List<CategoryRestaurantViewDto>
            {
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 1,
                    CategoryRestaurantName = "Italian",
                    IsDelete = false
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 2,
                    CategoryRestaurantName = "Japanese",
                    IsDelete = false
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 3,
                    CategoryRestaurantName = "Mexican",
                    IsDelete = true
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 4,
                    CategoryRestaurantName = "Chinese",
                    IsDelete = false
                }
            };

            // Giả lập trả về danh sách Category Restaurant
            _mockCategoryRestaurantService.Setup(service => service.GetAllCategoryRestaurantAsync())
                .ReturnsAsync(categoryList.Where(c => !c.IsDelete).ToList()); // Chỉ lấy các Category không bị xóa

            // Act
            var result = await _controller.GetAllCategoryRestaurants() as OkObjectResult;

            // Assert
            
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, result.StatusCode); // Kiểm tra mã trạng thái HTTP
            var response = result.Value as List<CategoryRestaurantViewDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response); // Kiểm tra dữ liệu trả về
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, response.Count); // Kiểm tra số lượng phần tử trả về (sau khi lọc IsDelete)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Italian", response[0].CategoryRestaurantName); // Kiểm tra tên Category Restaurant đầu tiên
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Chinese", response[2].CategoryRestaurantName); // Kiểm tra tên Category Restaurant cuối cùng
        }
    }
}
