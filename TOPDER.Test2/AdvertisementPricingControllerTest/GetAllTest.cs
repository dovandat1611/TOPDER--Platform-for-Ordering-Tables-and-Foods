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
using TOPDER.Service.Dtos.AdvertisementPricing;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.AdvertisementPricingControllerTest
{
    [TestClass]
    public class GetAllTest
    {
        private Mock<IAdvertisementPricingService> _mockAdvertisementPricingService;
        private AdvertisementPricingController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAdvertisementPricingService = new Mock<IAdvertisementPricingService>();
            _controller = new AdvertisementPricingController(_mockAdvertisementPricingService.Object);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOkResult_WithAdvertisementPricing()
        {
            // Arrange
            var mockAdvertisementPricings = new List<AdvertisementPricingDto>
            {
                new AdvertisementPricingDto { PricingId = 1, Price = 100 },
                new AdvertisementPricingDto { PricingId = 2, Price = 200 }
            };

            _mockAdvertisementPricingService
                .Setup(service => service.GetAllAdvertisementPricingAsync())
                .ReturnsAsync(mockAdvertisementPricings);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // Ensure it returns 200 OK status code
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(mockAdvertisementPricings, okResult.Value); // Ensure correct data is returned
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOkResult_IfNoAdvertisementPricing()
        {
            // Arrange
            var mockAdvertisementPricings = new List<AdvertisementPricingDto>();

            _mockAdvertisementPricingService
                .Setup(service => service.GetAllAdvertisementPricingAsync())
                .ReturnsAsync(mockAdvertisementPricings);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockAdvertisementPricings, okResult.Value);
        }
    }
}
