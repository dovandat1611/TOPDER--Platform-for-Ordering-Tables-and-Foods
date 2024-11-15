using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantRoomControllerTest
{
    [TestClass]
    public class IsEnabledBookingTest
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
        public async Task IsEnabledBooking_ShouldReturnBadRequest_WhenNoStateChange()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 1;
            bool isEnabledBooking = false; // Same as the initial state

            var existingRoom = new RestaurantRoom
            {
                RoomId = roomId,
                RestaurantId = restaurantId,
                IsBookingEnabled = false // Initial state matches the request
            };

            var existingRoomDto = new RestaurantRoomDto
            {
                RoomId = roomId,
                RestaurantId = restaurantId,
                IsBookingEnabled = false // Initial state matches the request
            };

            _restaurantRoomServiceMock.Setup(service => service.GetItemAsync(roomId, restaurantId))
                .ReturnsAsync(existingRoomDto);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có sự thay đổi trạng thái đặt phòng.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_ShouldReturnNotFound_WhenRoomNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 999; // Non-existent roomId
            bool isEnabledBooking = true;

            // Mocking the service method to return null (room not found)
            _restaurantRoomServiceMock.Setup(service => service.GetItemAsync(roomId, restaurantId))
                .ReturnsAsync((RestaurantRoomDto)null);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy phòng với Id {roomId}.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_ShouldReturnForbid_WhenUnauthorized()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 1;
            bool isEnabledBooking = true;

            var existingRoom = new RestaurantRoomDto
            {
                RoomId = roomId,
                RestaurantId = 2, // Room belongs to a different restaurant
                IsBookingEnabled = false // Initial state
            };

            // Mocking the service method to return a room with a different restaurantId
            _restaurantRoomServiceMock.Setup(service => service.GetItemAsync(roomId, restaurantId))
                .ReturnsAsync(existingRoom);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }
    }
}
