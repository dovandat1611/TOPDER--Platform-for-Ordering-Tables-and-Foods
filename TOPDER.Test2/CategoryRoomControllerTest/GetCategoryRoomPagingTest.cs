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
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.CategoryRoomControllerTest
{
    [TestClass]
    public class GetCategoryRoomPagingTest
    {
        private Mock<ICategoryRoomService> _mockCategoryRoomService;
        private CategoryRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Khởi tạo mocks và controller
            _mockCategoryRoomService = new Mock<ICategoryRoomService>();
            _controller = new CategoryRoomController(_mockCategoryRoomService.Object);
        }

        [TestMethod]
        public async Task GetCategoryRoomPaging_WithCategoryRoomNameNull_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string? categoryRoomName = null;

            var mockPaginatedResult = new PaginatedList<CategoryRoomDto>(
                new List<CategoryRoomDto> { new CategoryRoomDto { CategoryRoomId = 1, CategoryName = "Room A" } },
                1,
                pageNumber,
                pageSize
            );

            _mockCategoryRoomService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryRoomName))
                .ReturnsAsync(mockPaginatedResult);

            // Act
            var result = await _controller.GetCategoryRoomPaging(restaurantId, pageNumber, pageSize, categoryRoomName) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as PaginatedResponseDto<CategoryRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count); // Check that there's one item in the result
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Room A", response.Items.First().CategoryName); // Check that the name is correct
        }

        [TestMethod]
        public async Task GetCategoryRoomPaging_WithCategoryRoomNameNotNull_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string categoryRoomName = "Room B";

            var mockPaginatedResult = new PaginatedList<CategoryRoomDto>(
                new List<CategoryRoomDto> { new CategoryRoomDto { CategoryRoomId = 2, CategoryName = "Room B" } },
                1,
                pageNumber,
                pageSize
            );

            _mockCategoryRoomService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryRoomName))
                .ReturnsAsync(mockPaginatedResult);

            // Act
            var result = await _controller.GetCategoryRoomPaging(restaurantId, pageNumber, pageSize, categoryRoomName) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as PaginatedResponseDto<CategoryRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count); // Check that there's one item in the result
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Room B", response.Items.First().CategoryName); // Check that the name is correct
        }
    }
}
