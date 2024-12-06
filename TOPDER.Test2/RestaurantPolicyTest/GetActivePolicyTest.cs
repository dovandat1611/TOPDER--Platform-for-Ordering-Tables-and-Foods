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
    public class GetActivePolicyTest
    {
        private Mock<IRestaurantPolicyService> _mockRestaurantPolicyService;
        private RestaurantPolicyController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRestaurantPolicyService = new Mock<IRestaurantPolicyService>();
            _controller = new RestaurantPolicyController(_mockRestaurantPolicyService.Object);
        }

        [TestMethod]
        public async Task GetActivePolicy_PolicyFound_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            var mockPolicy = new RestaurantPolicyDto
            {
                PolicyId = 1,
                RestaurantId = restaurantId,
                FirstFeePercent = 5,
                ReturningFeePercent = 10,
                CancellationFeePercent = 15,
                Status = "Active",
                CreateDate = DateTime.Now
            };

            _mockRestaurantPolicyService
                .Setup(service => service.GetActivePolicyAsync(restaurantId))
                .ReturnsAsync(mockPolicy);

            // Act
            var result = await _controller.GetActivePolicy(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnedPolicy = okResult.Value as RestaurantPolicyDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedPolicy);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockPolicy.PolicyId, returnedPolicy.PolicyId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockPolicy.Status, returnedPolicy.Status);
        }

        [TestMethod]
        public async Task GetActivePolicy_PolicyNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = -1;

            _mockRestaurantPolicyService
                .Setup(service => service.GetActivePolicyAsync(restaurantId))
                .ReturnsAsync((RestaurantPolicyDto)null);

            // Act
            var result = await _controller.GetActivePolicy(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No active policy found for this restaurant.", notFoundResult.Value);
        }
    }
}
