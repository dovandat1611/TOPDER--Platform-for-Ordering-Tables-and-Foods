using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CategoryRoomControllerTest
{
    [TestClass]
    public class GetAllCategoryRoomsTest
    {
        private Mock<ICategoryRoomService> _mockCategoryRoomService;
        private CategoryRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCategoryRoomService = new Mock<ICategoryRoomService>();
            _controller = new CategoryRoomController(_mockCategoryRoomService.Object);
        }

        [TestMethod]
        public async Task GetAllCategoryRooms_WithValidRestaurantId_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            var categoryRooms = new List<CategoryRoomDto>
        {
            new CategoryRoomDto { CategoryRoomId = 1, CategoryName = "Luxury Room" },
            new CategoryRoomDto { CategoryRoomId = 2, CategoryName = "Standard Room" }
        };

            _mockCategoryRoomService.Setup(service => service.GetAllCategoryRoomAsync(restaurantId))
                .ReturnsAsync(categoryRooms); // Simulate successful data retrieval

            // Act
            var result = await _controller.GetAllCategoryRooms(restaurantId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<CategoryRoomDto>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(categoryRooms.Count, ((IEnumerable<CategoryRoomDto>)result.Value).Count());
        }


        [TestMethod]
        public async Task GetAllCategoryRooms_WithNegativeRestaurantId_ReturnsOK()
        {
            // Arrange
            int restaurantId = -1;
            var categoryRooms = new List<CategoryRoomDto>
        {
            new CategoryRoomDto { CategoryRoomId = 1, CategoryName = "Luxury Room" },
            new CategoryRoomDto { CategoryRoomId = 2, CategoryName = "Standard Room" }
        };

            _mockCategoryRoomService.Setup(service => service.GetAllCategoryRoomAsync(restaurantId))
                .ReturnsAsync(categoryRooms); // Simulate successful data retrieval

            // Act
            var result = await _controller.GetAllCategoryRooms(restaurantId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<CategoryRoomDto>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(categoryRooms.Count, ((IEnumerable<CategoryRoomDto>)result.Value).Count());
        }
    }
}
