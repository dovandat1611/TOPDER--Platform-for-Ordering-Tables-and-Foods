using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantRoomControllerTest
{
    [TestClass]
    public class GetItemTest
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
        public async Task GetItem_ShouldReturnOk_WhenRoomIsFound()
        {
            // Arrange
            var restaurantId = 1;
            var roomId = 1;
            var roomDto = new RestaurantRoomDto
            {
                RoomId = roomId,
                RestaurantId = restaurantId,
                RoomName = "VIP Room",
                MaxCapacity = 10,
                Description = "Luxury room"
            };

            _restaurantRoomServiceMock.Setup(service => service.GetItemAsync(roomId, restaurantId))
                .ReturnsAsync(roomDto);

            // Act
            var result = await _controller.GetItem(restaurantId, roomId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(roomDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetItem_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            var restaurantId = 1;
            var roomId = -1;

            _restaurantRoomServiceMock.Setup(service => service.GetItemAsync(roomId, restaurantId))
                .ThrowsAsync(new KeyNotFoundException($"Không tìm thấy bàn với Id {roomId}."));

            // Act
            var result = await _controller.GetItem(restaurantId, roomId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy bàn với Id {roomId}.", notFoundResult.Value);
        }

    }
}
