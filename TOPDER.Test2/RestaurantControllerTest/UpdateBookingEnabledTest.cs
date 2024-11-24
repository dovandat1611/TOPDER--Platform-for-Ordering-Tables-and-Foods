using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class UpdateBookingEnabledTest
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
        public async Task UpdateBookingEnabled_ShouldReturnOk_WhenStatusChanges()
        {
            // Arrange
            int restaurantId = 1;
            bool newBookingStatus = true;
            _restaurantServiceMock.Setup(service => service.IsEnabledBookingAsync(restaurantId, newBookingStatus))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBookingEnabled(restaurantId, newBookingStatus);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookingEnabled_ShouldReturnOk_WhenStatusNotChanged()
        {
            // Arrange
            int restaurantId = 1;
            bool currentBookingStatus = false;
            _restaurantServiceMock.Setup(service => service.IsEnabledBookingAsync(restaurantId, currentBookingStatus))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBookingEnabled(restaurantId, currentBookingStatus);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookingEnabled_ShouldReturnBadRequest_WhenRestaurantNotFound()
        {
            // Arrange
            int restaurantId = -1;
            bool newBookingStatus = true;
            _restaurantServiceMock.Setup(service => service.IsEnabledBookingAsync(restaurantId, newBookingStatus))
                .ThrowsAsync(new Exception("Restaurant not found."));

            // Act
            var result = await _controller.UpdateBookingEnabled(restaurantId, newBookingStatus);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

    }
}
