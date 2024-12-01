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

namespace TOPDER.Test2.PolicySystemControllerTest
{
    [TestClass]
    public class DeletePolicyTest
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
        public async Task DeletePolicy_ReturnsOk_WhenPolicyIsDeleted()
        {
            // Arrange: Mock the service to return true when RemoveAsync is called
            int policyId = 1;
            _mockPolicySystemService.Setup(service => service.RemoveAsync(policyId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePolicy(policyId);

            // Assert: Verify the result is OkObjectResult with a success message
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeletePolicy_ReturnsNotFound_WhenPolicyDoesNotExist()
        {
            // Arrange: Mock the service to return false when RemoveAsync is called
            int policyId = 999;
            _mockPolicySystemService.Setup(service => service.RemoveAsync(policyId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePolicy(policyId);

            // Assert: Verify the result is NotFoundObjectResult with the correct message
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Policy not found.", notFoundResult.Value);
        }
    }
}
