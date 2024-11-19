using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class UpdateRestaurantInfoTest
    {
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private RestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new RestaurantController(_restaurantServiceMock.Object, _cloudinaryServiceMock.Object); // Inject mock service
        }


        [TestMethod]
        public async Task UpdateRestaurantInfo_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange: Thêm lỗi vào ModelState
            _controller.ModelState.AddModelError("NameRes", "Tên nhà hàng không hợp lệ.");

            var restaurantDto = new UpdateInfoRestaurantDto
            {
                Uid = 1,
                NameOwner = "John Doe",
                NameRes = "Test Restaurant",
                Address = "123 Test St",
                Phone = "1234567890",
                MaxCapacity = 50,
                Price = 100m
            };

            // Act
            var result = await _controller.UpdateRestaurantInfo(restaurantDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRestaurantInfo_ReturnsNotFound_WhenRestaurantNotFound()
        {
            // Arrange: Mock return false for UpdateInfoRestaurantAsync (restaurant not found)
            var restaurantDto = new UpdateInfoRestaurantDto
            {
                Uid = 99991,
                NameOwner = "John Doe",
                NameRes = "Test Restaurant",
                Address = "123 Test St",
                Phone = "1234567890",
                MaxCapacity = 50,
                Price = 100m
            };

            _restaurantServiceMock
                .Setup(service => service.UpdateInfoRestaurantAsync(It.IsAny<UpdateInfoRestaurantDto>()))
                .ReturnsAsync(false); // Simulate restaurant not found

            // Act
            var result = await _controller.UpdateRestaurantInfo(restaurantDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult?.StatusCode);
        }

       
        [TestMethod]
        public async Task UpdateRestaurantInfo_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var restaurantDto = new UpdateInfoRestaurantDto
            {
                Uid = 1,
                NameOwner = "John Doe",
                NameRes = "Restaurant Name",
                Phone = "0123456789",
                Address = "123 Street",
                MaxCapacity = 50,
                Price = 100m,
                OpenTime = new TimeSpan(9, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0)
            };

            _restaurantServiceMock
                .Setup(service => service.UpdateInfoRestaurantAsync(It.IsAny<UpdateInfoRestaurantDto>()))
                .ReturnsAsync(true); // Simulate successful update

            // Act
            var result = await _controller.UpdateRestaurantInfo(restaurantDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        
    }
}
