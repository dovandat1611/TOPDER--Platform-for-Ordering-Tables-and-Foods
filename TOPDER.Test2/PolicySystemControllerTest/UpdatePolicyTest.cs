using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.PolicySystem;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.PolicySystemControllerTest
{
    [TestClass]
    public class UpdatePolicyTest
    {
        private Mock<IPolicySystemService> _mockPolicySystemService;
        private PolicySystemController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock for IPolicySystemService
            _mockPolicySystemService = new Mock<IPolicySystemService>();

            // Create an instance of the PolicySystemController with the mocked service
            _controller = new PolicySystemController(_mockPolicySystemService.Object);
        }
        [TestMethod]
        public async Task UpdatePolicy_ReturnsOk_WhenPolicyIsUpdated()
        {
            // Arrange
            var policyDto = new UpdatePolicySystemDto
            {
                PolicyId = 1,
                MinOrderValue = 100,
                MaxOrderValue = 500,
                FeeAmount = 50,
                Status = "Active"
            };

            _mockPolicySystemService.Setup(service => service.UpdateAsync(policyDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePolicy(policyDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdatePolicy_ReturnsNotFound_WhenPolicyDoesNotExist()
        {
            // Arrange
            var policyDto = new UpdatePolicySystemDto
            {
                PolicyId = -1,
                MinOrderValue = 100,
                MaxOrderValue = 500,
                FeeAmount = 50,
                Status = "Inactive"
            };

            _mockPolicySystemService.Setup(service => service.UpdateAsync(policyDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdatePolicy(policyDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Policy not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdatePolicy_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var policyDto = new UpdatePolicySystemDto(); // Invalid DTO
            _controller.ModelState.AddModelError("MinOrderValue", "MinOrderValue is required.");

            // Act
            var result = await _controller.UpdatePolicy(policyDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult.Value);
        }
    }
}
