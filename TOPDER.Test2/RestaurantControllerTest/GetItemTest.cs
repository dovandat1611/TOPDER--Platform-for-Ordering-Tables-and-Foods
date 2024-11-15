using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class GetItemTest
    {
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;

        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private RestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new RestaurantController(_restaurantServiceMock.Object, _cloudinaryServiceMock.Object); // Inject mock service
        }
        [TestMethod]
        public async Task GetItem_ReturnsOkResult_WithRestaurantDetails()
        {
            // Arrange
            var restaurantId = 1;

            // Act
            var result = await _controller.GetItem(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetItem_ReturnsNotFound_WhenRestaurantDoesNotExist()
        {
            // Arrange
            var restaurantId = 99999;
            _restaurantServiceMock
                .Setup(service => service.GetItemAsync(restaurantId))
                .ThrowsAsync(new KeyNotFoundException("Restaurant not found"));

            // Act
            var result = await _controller.GetItem(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetItem_ReturnsBadRequest_WhenInvalidOperationOccurs()
        {
            // Arrange
            var restaurantId = -1; // Invalid ID
            _restaurantServiceMock
                .Setup(service => service.GetItemAsync(restaurantId))
                .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.GetItem(restaurantId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Invalid operation", badRequestResult.Value);
        }
    }
}
