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
    public class UpdateTest
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
        public async Task Update_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                PricingId = 1,
                AdminId = 1,
                PricingName = "Updated Plan",
                Description = "Updated description.",
                DurationHours = 48,
                Price = 200.00M
            };

            _advertisementPricingServiceMock
                .Setup(s => s.UpdateAsync(advertisementPricingDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing updated successfully.", okResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.UpdateAsync(advertisementPricingDto), Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenAdvertisementPricingNotFound()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                PricingId = 1,
                AdminId = 1,
                PricingName = "Updated Plan",
                Description = "Updated description.",
                DurationHours = 48,
                Price = 200.00M
            };

            _advertisementPricingServiceMock
                .Setup(s => s.UpdateAsync(advertisementPricingDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing not found.", notFoundResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.UpdateAsync(advertisementPricingDto), Times.Once);
        }

        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var advertisementPricingDto = new AdvertisementPricingDto
            {
                PricingId = 1,
                AdminId = 1,
                PricingName = "", // Invalid: PricingName is required
                Description = "Updated description.",
                DurationHours = 48,
                Price = 200.00M
            };

            _controller.ModelState.AddModelError("PricingName", "The PricingName field is required.");

            // Act
            var result = await _controller.Update(advertisementPricingDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(badRequestResult.Value is SerializableError);

            _advertisementPricingServiceMock.Verify(s => s.UpdateAsync(It.IsAny<AdvertisementPricingDto>()), Times.Never);
        }
    }
}
