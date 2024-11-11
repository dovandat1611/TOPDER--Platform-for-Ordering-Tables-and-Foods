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
    public class GetCategoryRoomTest
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
        public async Task GetCategoryRoom_WithInvalidRestaurantId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId
            int categoryRoomId = 1; // Valid categoryRoomId

            _mockCategoryRoomService.Setup(service => service.GetItemAsync(categoryRoomId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException()); // Simulate not found

            // Act
            var result = await _controller.GetCategoryRoom(restaurantId, categoryRoomId) as NotFoundObjectResult;

            // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Room với ID {categoryRoomId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task GetCategoryRoom_WithValidRestaurantIdAndInvalidCategoryRoomId_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            int categoryRoomId = -1; // Invalid categoryRoomId

            _mockCategoryRoomService.Setup(service => service.GetItemAsync(categoryRoomId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException()); // Simulate not found

            // Act
            var result = await _controller.GetCategoryRoom(restaurantId, categoryRoomId) as NotFoundObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Category Room với ID {categoryRoomId} không tồn tại.", result.Value);
        }

        [TestMethod]
        public async Task GetCategoryRoom_WithValidData_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1; // Valid restaurantId
            int categoryRoomId = 1; // Valid categoryRoomId
            var categoryRoom = new CategoryRoomDto { CategoryRoomId = categoryRoomId, CategoryName = "Luxury Room" };

            _mockCategoryRoomService.Setup(service => service.GetItemAsync(categoryRoomId, restaurantId))
                .ReturnsAsync(categoryRoom); // Simulate found category room

            // Act
            var result = await _controller.GetCategoryRoom(restaurantId, categoryRoomId) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(categoryRoom, result.Value);
        }

        //[TestMethod]
        //public async Task GetCategoryRoom_WithUnauthorizedAccess_ReturnsInternalServerError()
        //{
        //    // Arrange
        //    int restaurantId = -1; // Invalid restaurantId
        //    int categoryRoomId = 1; // Valid categoryRoomId

        //    _mockCategoryRoomService.Setup(service => service.GetItemAsync(categoryRoomId, restaurantId))
        //        .ThrowsAsync(new UnauthorizedAccessException()); // Simulate unauthorized access

        //    // Act
        //    var result = await _controller.GetCategoryRoom(restaurantId, categoryRoomId) as ObjectResult;

        //    // Assert
        //    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result); // Ensure that result is not null
        //}
    }
}
