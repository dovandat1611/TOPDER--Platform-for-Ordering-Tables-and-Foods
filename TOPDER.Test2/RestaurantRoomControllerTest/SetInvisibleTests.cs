using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantRoomControllerTest
{

    [TestClass]
    public class SetInvisibleTests
    {
        private Mock<IRestaurantRoomService> _restaurantRoomServiceMock;
        private RestaurantRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            _restaurantRoomServiceMock = new Mock<IRestaurantRoomService>();
            _controller = new RestaurantRoomController(_restaurantRoomServiceMock.Object);
        }

        [TestMethod]
        public async Task SetInvisible_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 999; // Non-existent room
            _restaurantRoomServiceMock.Setup(service => service.InvisibleAsync(roomId, restaurantId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, roomId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy phòng để xóa.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task SetInvisible_ShouldReturnOk_WhenRoomIsSuccessfullyHidden()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 1; // Valid room
            _restaurantRoomServiceMock.Setup(service => service.InvisibleAsync(roomId, restaurantId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SetInvisible(restaurantId, roomId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual($"Ẩn/Xóa Room và các bảng liên quan thành công.", okResult.Value);
        }

    }
}
