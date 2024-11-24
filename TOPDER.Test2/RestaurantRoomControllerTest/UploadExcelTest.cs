using Microsoft.AspNetCore.Http;
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
    public class UploadExcelTest
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
        public async Task UploadExcel_ValidFile_ReturnsOk()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "Sample Excel Content";
            var fileName = "rooms.xlsx";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            mockFile.Setup(_ => _.OpenReadStream()).Returns(stream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(stream.Length);

            var createDto = new CreateExcelRestaurantRoomDto
            {
                RestaurantId = 1,
                File = mockFile.Object
            };

            _restaurantRoomServiceMock
                .Setup(service => service.AddRangeExcelAsync(It.IsAny<CreateExcelRestaurantRoomDto>()))
                .ReturnsAsync((true, "Rooms created successfully."));

            // Act
            var result = await _controller.UploadExcel(createDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UploadExcel_InvalidFile_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateExcelRestaurantRoomDto
            {
                RestaurantId = 1,
                File = null // File is missing
            };

            // Act
            var result = await _controller.UploadExcel(createDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UploadExcel_NonExistingRestaurantId_ReturnsBadRequest()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "Sample Excel Content";
            var fileName = "rooms.xlsx";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            mockFile.Setup(_ => _.OpenReadStream()).Returns(stream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(stream.Length);

            var createDto = new CreateExcelRestaurantRoomDto
            {
                RestaurantId = 99999, // Non-existing RestaurantId
                File = mockFile.Object
            };

            _restaurantRoomServiceMock
                .Setup(service => service.AddRangeExcelAsync(It.IsAny<CreateExcelRestaurantRoomDto>()))
                .ReturnsAsync((false, "RestaurantId does not exist."));

            // Act
            var result = await _controller.UploadExcel(createDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

    }
}
