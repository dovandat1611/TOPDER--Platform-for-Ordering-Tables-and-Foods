using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.AdvertisementPricingControllerTest
{
    [TestClass]
    public class DeleteTest
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
        public async Task Delete_ShouldReturnOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            int pricingId = 1;
            _advertisementPricingServiceMock
                .Setup(s => s.RemoveAsync(pricingId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(pricingId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing deleted successfully.", okResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.RemoveAsync(pricingId), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFound_WhenAdvertisementPricingNotFound()
        {
            // Arrange
            int pricingId = 999; // Non-existent ID
            _advertisementPricingServiceMock
                .Setup(s => s.RemoveAsync(pricingId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(pricingId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement Pricing not found.", notFoundResult.Value);

            _advertisementPricingServiceMock.Verify(s => s.RemoveAsync(pricingId), Times.Once);
        }
    }
}
