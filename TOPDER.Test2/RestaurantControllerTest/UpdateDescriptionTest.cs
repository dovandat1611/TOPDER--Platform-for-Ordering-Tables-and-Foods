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
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class UpdateDescriptionTest
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
        public async Task UpdateDescription_Success()
        {
            // Arrange
            var restaurantId = 1;
            var description = "New Description";
            var subDescription = "New Subdescription";

            var restaurant = new Restaurant { Uid = restaurantId, Description = "Old Description", Subdescription = "Old Subdescription" };

            // Setup the mock service to return a valid restaurant
            _restaurantServiceMock.Setup(r => r.UpdateDescriptionAsync(restaurantId, description, subDescription)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDescription(restaurantId, description, subDescription);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDescription_NoChangesMade()
        {
            // Arrange
            var restaurantId = 1;
            string? description = null;
            string? subDescription = null;

            var restaurant = new Restaurant { Uid = restaurantId, Description = "Old Description", Subdescription = "Old Subdescription" };
            var restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            var restaurantServiceMock = new Mock<IRestaurantService>();

            restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            restaurantRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Restaurant>())).ReturnsAsync(false);  // No update

            // Act
            var result = await _controller.UpdateDescription(restaurantId, description, subDescription);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDescription_RestaurantNotFound()
        {
            // Arrange
            var restaurantId = 99999; // Assuming this ID doesn't exist
            var description = "New Description";
            var subDescription = "New Subdescription";

            var restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            var restaurantServiceMock = new Mock<IRestaurantService>();

            restaurantRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null);  // Not found

            // Act
            var result = await _controller.UpdateDescription(restaurantId, description, subDescription);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
                        Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDescription_SubDescriptionIsNull_ShouldUpdateDescriptionOnly()
        {
            // Arrange
            var restaurantId = 1;
            var description = "New Description";
            var subDescription = "";

            var restaurant = new Restaurant { Uid = restaurantId, Description = "Old Description", Subdescription = "Old Subdescription" };

            // Setup the mock service to return a valid restaurant
            _restaurantServiceMock.Setup(r => r.UpdateDescriptionAsync(restaurantId, description, subDescription)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDescription(restaurantId, description, subDescription);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDescription_DescriptionIsNull_ShouldUpdateDescriptionOnly()
        {
            // Arrange
            var restaurantId = 1;
            var description = "";
            var subDescription = "New Subdescription";

            var restaurant = new Restaurant { Uid = restaurantId, Description = "Old Description", Subdescription = "Old Subdescription" };

            // Setup the mock service to return a valid restaurant
            _restaurantServiceMock.Setup(r => r.UpdateDescriptionAsync(restaurantId, description, subDescription)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDescription(restaurantId, description, subDescription);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

    }
}
