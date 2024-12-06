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

namespace TOPDER.Test2.RestaurantPolicyTest
{
    [TestClass]
    public class ChoosePolicyToUseTest
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
        public async Task ChoosePolicyToUse_PolicyActivated_ReturnsOk()
        {
            // Arrange
            int policyId = 1;

            _mockRestaurantPolicyService
                .Setup(service => service.ChoosePolicyToUseAsync(policyId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ChoosePolicyToUse(policyId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task ChoosePolicyToUse_PolicyNotFound_ReturnsNotFound()
        {
            // Arrange
            int policyId = -1;

            _mockRestaurantPolicyService
                .Setup(service => service.ChoosePolicyToUseAsync(policyId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.ChoosePolicyToUse(policyId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Policy not found or cannot be activated.", notFoundResult.Value);
        }
    }
}
