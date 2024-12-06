using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.RestaurantPolicy;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantPolicyTest
{
    [TestClass]
    public class CreatePolicyTest
    {
        private Mock<IRestaurantPolicyService> _mockRestaurantPolicyService;
        private RestaurantPolicyController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRestaurantPolicyService = new Mock<IRestaurantPolicyService>();
            _controller = new RestaurantPolicyController(_mockRestaurantPolicyService.Object);

            // Giả lập ModelState hợp lệ
            _controller.ModelState.Clear();
        }

        [TestMethod]
        public async Task CreatePolicy_FirstFeePercentInvalid_ReturnsBadRequest()
        {
            // Arrange
            var restaurantPolicyDto = new CreateRestaurantPolicyDto
            {
                RestaurantId = 1
            };
            _controller.ModelState.AddModelError("FirstFeePercent", "FirstFeePercent is required.");

            // Act
            var result = await _controller.CreatePolicy(restaurantPolicyDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreatePolicy_ReturningFeePercentInvalid_ReturnsBadRequest()
        {
            // Arrange
            var restaurantPolicyDto = new CreateRestaurantPolicyDto
            {
                RestaurantId = 1
            };
            _controller.ModelState.AddModelError("ReturningFeePercent", "ReturningFeePercent is required.");

            // Act
            var result = await _controller.CreatePolicy(restaurantPolicyDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }


        [TestMethod]
        public async Task CreatePolicy_CancellationFeePercentInvalid_ReturnsBadRequest()
        {
            // Arrange
            var restaurantPolicyDto = new CreateRestaurantPolicyDto
            {
                RestaurantId = 1
            };
            _controller.ModelState.AddModelError("CancellationFeePercent", "CancellationFeePercent is required.");

            // Act
            var result = await _controller.CreatePolicy(restaurantPolicyDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreatePolicy_Successful_ReturnsOk()
        {
            // Arrange
            var restaurantPolicyDto = new CreateRestaurantPolicyDto
            {
                RestaurantId = 1,
                FirstFeePercent = 5,
                ReturningFeePercent = 10,
                CancellationFeePercent = 15
            };

            _mockRestaurantPolicyService
                .Setup(service => service.AddAsync(restaurantPolicyDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreatePolicy(restaurantPolicyDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task CreatePolicy_Failure_ReturnsInternalServerError()
        {
            // Arrange
            var restaurantPolicyDto = new CreateRestaurantPolicyDto
            {
                RestaurantId = 1,
                FirstFeePercent = 5,
                ReturningFeePercent = 10,
                CancellationFeePercent = 15
            };

            _mockRestaurantPolicyService
                .Setup(service => service.AddAsync(restaurantPolicyDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePolicy(restaurantPolicyDto);

            // Assert
                var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while creating the restaurant policy.", statusCodeResult.Value);
        }
    }
}
