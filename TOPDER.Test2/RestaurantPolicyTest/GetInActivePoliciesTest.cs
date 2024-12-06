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
    public class GetInActivePoliciesTest
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
        public async Task GetInActivePolicies_PoliciesFound_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            var mockPolicies = new List<RestaurantPolicyDto>
        {
            new RestaurantPolicyDto
            {
                PolicyId = 1,
                RestaurantId = restaurantId,
                FirstFeePercent = 5,
                ReturningFeePercent = 10,
                CancellationFeePercent = 15,
                Status = "Inactive",
                CreateDate = DateTime.Now.AddDays(-10)
            },
            new RestaurantPolicyDto
            {
                PolicyId = 2,
                RestaurantId = restaurantId,
                FirstFeePercent = 8,
                ReturningFeePercent = 12,
                CancellationFeePercent = 20,
                Status = "Inactive",
                CreateDate = DateTime.Now.AddDays(-5)
            }
        };

            _mockRestaurantPolicyService
                .Setup(service => service.GetInActivePolicyAsync(restaurantId))
                .ReturnsAsync(mockPolicies);

            // Act
            var result = await _controller.GetInActivePolicies(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnedPolicies = okResult.Value as List<RestaurantPolicyDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedPolicies);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(mockPolicies.Count, returnedPolicies.Count);
        }

        [TestMethod]
        public async Task GetInActivePolicies_NoPoliciesFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = -1;

            _mockRestaurantPolicyService
                .Setup(service => service.GetInActivePolicyAsync(restaurantId))
                .ReturnsAsync(new List<RestaurantPolicyDto>());

            // Act
            var result = await _controller.GetInActivePolicies(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No inactive policies found for this restaurant.", notFoundResult.Value);
        }

    }
}
