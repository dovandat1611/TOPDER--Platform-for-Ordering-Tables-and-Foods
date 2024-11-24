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
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.RestaurantRoomControllerTest
{
    [TestClass]
    public class SearchPagingTest
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
        public async Task SearchPaging_ShouldReturnOkWithFilteredResultsByRoomName()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            string roomName = "roomName";

            var paginatedList = new PaginatedList<RestaurantRoomDto>(
                new List<RestaurantRoomDto>
                {
            new RestaurantRoomDto { RoomId = 1, RestaurantId = restaurantId, RoomName = "roomName", MaxCapacity = 15 },
            new RestaurantRoomDto { RoomId = 2, RestaurantId = restaurantId, RoomName = "roomName", MaxCapacity = 20 }
                },
                2, pageNumber, pageSize
            );

            _restaurantRoomServiceMock.Setup(service =>
                    service.GetRoomListAsync(pageNumber, pageSize, restaurantId, null, roomName))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPaging(restaurantId, pageNumber, pageSize, null, roomName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count);

            // Verify the first item's properties
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items[0].RoomId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("roomName", response.Items[0].RoomName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(15, response.Items[0].MaxCapacity);

            // Verify the second item's properties
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items[1].RoomId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("roomName", response.Items[1].RoomName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(20, response.Items[1].MaxCapacity);
        }

        [TestMethod]
        public async Task SearchPaging_ShouldReturnOkWithFilteredResults()
        {
            // Arrange
            int restaurantId = 1;
            int roomId = 1;
            int pageNumber = 1;
            int pageSize = 10;

            var paginatedList = new PaginatedList<RestaurantRoomDto>(
                new List<RestaurantRoomDto>
                {
            new RestaurantRoomDto { RoomId = 2, RestaurantId = restaurantId, RoomName = "Room B", MaxCapacity = 20 }
                },
                1, pageNumber, pageSize
            );

            _restaurantRoomServiceMock.Setup(service => service.GetRoomListAsync(pageNumber, pageSize, restaurantId, roomId, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPaging(restaurantId, pageNumber, pageSize, roomId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items[0].RoomId);
        }

        

        [TestMethod]
        public async Task SearchPaging_ShouldReturnOkWithFilteredResultsByRoomIdAndRoomName()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            int roomId = 2; // RoomId is not null
            string roomName = "roomName"; // RoomName is not null

            var paginatedList = new PaginatedList<RestaurantRoomDto>(
                new List<RestaurantRoomDto>
                {
            new RestaurantRoomDto { RoomId = 2, RestaurantId = restaurantId, RoomName = "roomName", MaxCapacity = 20 },
            new RestaurantRoomDto { RoomId = 3, RestaurantId = restaurantId, RoomName = "roomName", MaxCapacity = 25 }
                },
                2, pageNumber, pageSize
            );

            // Mocking the service call to return the filtered data
            _restaurantRoomServiceMock.Setup(service =>
                    service.GetRoomListAsync(pageNumber, pageSize, restaurantId, roomId, roomName))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPaging(restaurantId, pageNumber, pageSize, roomId, roomName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count); // Verifying two items are returned

            // Check that the filtered rooms match the expected values
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items[0].RoomId); // RoomId should match the requested ID
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("roomName", response.Items[0].RoomName); // RoomName should match the requested name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(20, response.Items[0].MaxCapacity);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, response.Items[1].RoomId); // The second room should also match the requested name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("roomName", response.Items[1].RoomName); // RoomName should match the requested name
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(25, response.Items[1].MaxCapacity);
        }

        [TestMethod]
        public async Task SearchPaging_ShouldReturnOkWithAllRooms_WhenRoomIdAndRoomNameAreNull()
        {
            // Arrange
            int restaurantId = 1;
            int pageNumber = 1;
            int pageSize = 10;
            int? roomId = null; // roomId is null
            string? roomName = null; // roomName is null

            var paginatedList = new PaginatedList<RestaurantRoomDto>(
                new List<RestaurantRoomDto>
                {
            new RestaurantRoomDto { RoomId = 1, RestaurantId = restaurantId, RoomName = "Room A", MaxCapacity = 20 },
            new RestaurantRoomDto { RoomId = 2, RestaurantId = restaurantId, RoomName = "Room B", MaxCapacity = 25 }
                },
                2, pageNumber, pageSize
            );

            // Mocking the service call to return all rooms (no filtering by roomId or roomName)
            _restaurantRoomServiceMock.Setup(service =>
                    service.GetRoomListAsync(pageNumber, pageSize, restaurantId, roomId, roomName))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.SearchPaging(restaurantId, pageNumber, pageSize, roomId, roomName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantRoomDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count); // Verifying two rooms are returned

            // Verify the rooms match the expected values
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items[0].RoomId); // First room should have RoomId = 1
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Room A", response.Items[0].RoomName); // First room name should be "Room A"
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(20, response.Items[0].MaxCapacity); // MaxCapacity for first room is 20

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items[1].RoomId); // Second room should have RoomId = 2
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Room B", response.Items[1].RoomName); // Second room name should be "Room B"

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(25, response.Items[1].MaxCapacity); // MaxCapacity for second room is 25
        }

    }
}
