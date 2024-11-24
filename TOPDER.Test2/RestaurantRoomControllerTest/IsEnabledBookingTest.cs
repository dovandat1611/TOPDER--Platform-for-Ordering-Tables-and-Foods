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
        private Mock<IRestaurantRoomService> _mockService;
        private RestaurantRoomController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IRestaurantRoomService>();
            _controller = new RestaurantRoomController(_mockService.Object);
        }

        [TestMethod]
        public async Task IsEnabledBooking_SuccessfulUpdate_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 101;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Thay đổi trạng thái IsEnabledBooking: {isEnabledBooking} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_NoStateChange_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 101;
            bool isEnabledBooking = false;

            _mockService.Setup(s => s.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có sự thay đổi trạng thái đặt phòng.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_RoomNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = -1;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking))
                        .ThrowsAsync(new KeyNotFoundException("Không tìm thấy phòng với Id -1."));

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy phòng với Id 999.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_UnauthorizedAccess_ReturnsForbid()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 101;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking))
                        .ThrowsAsync(new UnauthorizedAccessException("Phòng với Id 101 không thuộc về nhà hàng với Id 1."));

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, roomId, isEnabledBooking);

            // Assert
            var forbidResult = result as ForbidResult;

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }
    }
}
