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
    public class UpdateTest
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
        public async Task Update_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updateDto = new UpdateRestaurantRoomDto
            {
                RoomId = 1,
                CategoryRoomId = 2,
                RoomName = "Updated Room",
                MaxCapacity = 20,
                Description = "Updated description"
            };

            _restaurantRoomServiceMock.Setup(service => service.UpdateAsync(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update Room thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateRestaurantRoomDto
            {
                RoomId = -1,
                CategoryRoomId = null,
                RoomName = "Updated Room",
                MaxCapacity = 15,
                Description = null
            };

            _restaurantRoomServiceMock.Setup(service => service.UpdateAsync(updateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy phòng để cập nhật.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenRoomNameIsNull()
        {
            var updateDto = new UpdateRestaurantRoomDto
            {
                RoomId = 1,
                CategoryRoomId = 2,
                RoomName = "",
                MaxCapacity = 20,
                Description = "Updated description"
            };

            _restaurantRoomServiceMock.Setup(service => service.UpdateAsync(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update Room thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldHandleNullDescription()
        {
            // Arrange
            var updateDto = new UpdateRestaurantRoomDto
            {
                RoomId = 1,
                CategoryRoomId = 2,
                RoomName = "Valid Room Name",
                MaxCapacity = 20,
                Description = null // Null field
            };

            _restaurantRoomServiceMock.Setup(service => service.UpdateAsync(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update Room thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Update_ShouldHandleNullCategoryRoomId()
        {
            // Arrange
            var updateDto = new UpdateRestaurantRoomDto
            {
                RoomId = 1,
                CategoryRoomId = null, // Null field
                RoomName = "Valid Room Name",
                MaxCapacity = 20,
                Description = "Valid Description"
            };

            _restaurantRoomServiceMock.Setup(service => service.UpdateAsync(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Update Room thành công", okResult.Value);
        }
    }
}
