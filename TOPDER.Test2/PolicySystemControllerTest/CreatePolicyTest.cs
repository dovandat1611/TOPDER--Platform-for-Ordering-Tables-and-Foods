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
    public class CreatePolicyTest
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
        public async Task CreatePolicy_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange: Create an invalid DTO (e.g., missing required properties)
            var invalidPolicySystemDto = new CreatePolicySystemDto();
            _controller.ModelState.AddModelError("MinOrderValue", "MinOrderValue is required.");

            // Act
            var result = await _controller.CreatePolicy(invalidPolicySystemDto);

            // Assert: Verify that a BadRequest result is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(badRequestResult.StatusCode, 400); // BadRequest
        }

        [TestMethod]
        public async Task CreatePolicy_ReturnsBadRequest_WhenFeeAmountIsNegative()
        {
            // Arrange: Create an invalid DTO (e.g., missing required properties)
            // Arrange: Create a valid DTO
            var policySystemDto = new CreatePolicySystemDto
            {
                AdminId = 1,
                MinOrderValue = 50.00m,
                MaxOrderValue = 200.00m,
                FeeAmount = -1
            };
            _controller.ModelState.AddModelError("FeeAmount", "FeeAmount is required.");

            // Act
            var result = await _controller.CreatePolicy(policySystemDto);

            // Assert: Verify that a BadRequest result is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(badRequestResult.StatusCode, 400); // BadRequest
        }

        [TestMethod]
        public async Task CreatePolicy_ReturnsBadRequest_WhenMaxOrderValueIsNegative()
        {
            // Arrange: Create an invalid DTO (e.g., missing required properties)
            // Arrange: Create a valid DTO
            var policySystemDto = new CreatePolicySystemDto
            {
                AdminId = 1,
                MinOrderValue = 50,
                MaxOrderValue = -1,
                FeeAmount = 10
            };
            _controller.ModelState.AddModelError("MaxOrderValue", "MaxOrderValue is required.");

            // Act
            var result = await _controller.CreatePolicy(policySystemDto);

            // Assert: Verify that a BadRequest result is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(badRequestResult.StatusCode, 400); // BadRequest
        }

        [TestMethod]
        public async Task CreatePolicy_ReturnsOk_WhenPolicyCreatedSuccessfully()
        {
            // Arrange: Create a valid DTO
            var policySystemDto = new CreatePolicySystemDto
            {
                AdminId = 1,
                MinOrderValue = 50.00m,
                MaxOrderValue = 200.00m,
                FeeAmount = 10.00m
            };

            // Mock the service's AddAsync method to return true, indicating success
            _mockPolicySystemService.Setup(service => service.AddAsync(policySystemDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreatePolicy(policySystemDto);

            // Assert: Verify that an OkObjectResult is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(okResult.StatusCode, 200); // Ok response
        }

        [TestMethod]
        public async Task CreatePolicy_ReturnsInternalServerError_WhenPolicyCreationFails()
        {
            // Arrange: Create a valid DTO
            var policySystemDto = new CreatePolicySystemDto
            {
                AdminId = 1,
                MinOrderValue = 50.00m,
                MaxOrderValue = 200.00m,
                FeeAmount = 10.00m
            };

            // Mock the service's AddAsync method to return false, indicating failure
            _mockPolicySystemService.Setup(service => service.AddAsync(policySystemDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePolicy(policySystemDto);

            // Assert: Verify that an InternalServerError is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(objectResult.StatusCode, 500); // Internal Server Error
        }

        [TestMethod]
        public async Task CreatePolicy_ReturnsInternalServerError_WhenMinOrderValueThanMaxOrderValue()
        {
            // Arrange: Create a valid DTO
            var policySystemDto = new CreatePolicySystemDto
            {
                AdminId = 1,
                MinOrderValue = 2000.00m,
                MaxOrderValue = 50.00m,
                FeeAmount = 10.00m
            };

            // Mock the service's AddAsync method to return false, indicating failure
            _mockPolicySystemService.Setup(service => service.AddAsync(policySystemDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePolicy(policySystemDto);

            // Assert: Verify that an InternalServerError is returned
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(objectResult.StatusCode, 500); // Internal Server Error
        }
    }
}
