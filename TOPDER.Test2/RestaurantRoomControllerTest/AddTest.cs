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
    public class AddTest
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
        public async Task Add_ShouldReturnOk_WhenRoomAddedSuccessfully()
        {
            // Arrange
            var validDto = new CreateRestaurantRoomDto
            {
                RestaurantId = 1,
                CategoryRoomId = 0,
                RoomName = "VIP Room",
                MaxCapacity = 10,
                Description = "A premium room"
            };

            _restaurantRoomServiceMock.Setup(service => service.AddAsync(validDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Add(validDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo phòng thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Add_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidDto = new CreateRestaurantRoomDto
            {
                RestaurantId = -1, // Invalid: RestaurantId cannot be 0
                RoomName = null!, // Invalid: RoomName is required
                MaxCapacity = -1, // Invalid: MaxCapacity cannot be negative
            };

            _controller.ModelState.AddModelError("RestaurantId", "The field RestaurantId is required.");
            _controller.ModelState.AddModelError("RoomName", "The field RoomName is required.");
            _controller.ModelState.AddModelError("MaxCapacity", "MaxCapacity must be a positive number.");

            // Act
            var result = await _controller.Add(invalidDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }

        [TestMethod]
        public async Task Add_ShouldReturnOk_WhenDescriptionIsNull()
        {
            // Arrange
            var validDto = new CreateRestaurantRoomDto
            {
                RestaurantId = 1,
                RoomName = "VIP Room",
                MaxCapacity = 10,
                Description = null // Valid, as Description is optional
            };

            _restaurantRoomServiceMock.Setup(service => service.AddAsync(validDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Add(validDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo phòng thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Add_ShouldReturnBadRequest_WhenMaxCapacityIsInvalid()
        {
            // Arrange
            var invalidDto = new CreateRestaurantRoomDto
            {
                RestaurantId = 1,
                RoomName = "VIP Room",
                MaxCapacity = -1, // Invalid
                Description = "Test Room"
            };

            _controller.ModelState.AddModelError("MaxCapacity", "MaxCapacity must be a positive number.");

            // Act
            var result = await _controller.Add(invalidDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }
        [TestMethod]
        public async Task Add_ShouldReturnBadRequest_WhenRoomNameIsNull()
        {
            // Arrange
            var invalidDto = new CreateRestaurantRoomDto
            {
                RestaurantId = 1,
                RoomName = null!, // Invalid
                MaxCapacity = 10,
                Description = "Test Room"
            };

            _controller.ModelState.AddModelError("RoomName", "The field RoomName is required.");

            // Act
            var result = await _controller.Add(invalidDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }

    }
}
