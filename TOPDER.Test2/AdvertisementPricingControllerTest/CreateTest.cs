using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.AdvertisementPricingControllerTest
{
    [TestClass]
    public class CreateTest
    {
        private Mock<IAdvertisementPricingService> _advertisementPricingServiceMock;
        private AdvertisementPricingController _controller;

        [TestInitialize]
        public void Setup()
        {
            _advertisementPricingServiceMock = new Mock<IAdvertisementPricingService>();
            _controller = new AdvertisementPricingController(_advertisementPricingServiceMock.Object);
        }

        [TestMethod]
        public async Task Create_ShouldReturnOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                AdminId = 1,
                PricingName = "Standard Plan",
                Description = "This is a test plan.",
                DurationHours = 24,
                Price = 100.00M
            };

            _advertisementPricingServiceMock
                .Setup(s => s.AddAsync(advertisementPricingDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing created successfully.", okResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(advertisementPricingDto), Times.Once);
        }

        [TestMethod]
        public async Task Create_ShouldReturnOk_WhenDescriptionIsNull()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                AdminId = 1,
                PricingName = "Standard Plan",
                DurationHours = 24,
                Price = 100.00M
            };

            _advertisementPricingServiceMock
                .Setup(s => s.AddAsync(advertisementPricingDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing created successfully.", okResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(advertisementPricingDto), Times.Once);
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenServiceFailsToCreate()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                PricingId = 1,
                AdminId = 1,
                PricingName = "Standard Plan",
                Description = "This is a test plan.",
                DurationHours = 24,
                Price = 100.00M
            };

            _advertisementPricingServiceMock
                .Setup(s => s.AddAsync(advertisementPricingDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to create Advertisement Pricing.", badRequestResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(advertisementPricingDto), Times.Once);
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenAdminIdIsZero()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                AdminId = -1, // Invalid
                PricingName = "Standard Plan",
                Description = "This is a test plan.",
                DurationHours = 24,
                Price = 100.00M
            };

            _controller.ModelState.AddModelError("AdminId", "The AdminId field is required.");

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(badRequestResult.Value is SerializableError);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(It.IsAny<AdvertisementPricingDto>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenDurationHoursIsMissingOrNegative()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                AdminId = 1,
                PricingName = "Test Plan",
                Description = "This is a test plan.",
                DurationHours = -1, // Invalid: DurationHours is negative
                Price = 100.00M
            };

            _controller.ModelState.AddModelError("DurationHours", "The DurationHours field must be greater than 0.");

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(badRequestResult.Value is SerializableError);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(It.IsAny<AdvertisementPricingDto>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenPriceIsNegativeOrZero()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                AdminId = 1,
                PricingName = "Test Plan",
                Description = "This is a test plan.",
                DurationHours = 24,
                Price = 0.00M // Invalid: Price is zero
            };

            _controller.ModelState.AddModelError("Price", "The Price must be greater than 0.");

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(badRequestResult.Value is SerializableError);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(It.IsAny<AdvertisementPricingDto>()), Times.Never);
        }


        [TestMethod]
        public async Task Create_ShouldReturnBadRequest_WhenPricingNameEmpty()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                PricingId = 1,
                AdminId = 1,
                PricingName = "", // Invalid: PricingName is required
                Description = "This is a test plan.",
                DurationHours = 24,
                Price = 100.00M
            };

            _controller.ModelState.AddModelError("PricingName", "The PricingName field is required.");

            // Act
            var result = await _controller.Create(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(badRequestResult.Value is SerializableError);

            _advertisementPricingServiceMock.Verify(s => s.AddAsync(It.IsAny<AdvertisementPricingDto>()), Times.Never);
        }

        
    }
}
