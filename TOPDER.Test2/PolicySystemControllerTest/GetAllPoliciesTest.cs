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
    public class GetAllPoliciesTest
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
        public async Task GetAllPolicies_ReturnsOkResult_WithListOfPolicies()
        {
            // Arrange
            var mockPolicies = new List<PolicySystemDto>
        {
            new PolicySystemDto
            {
                PolicyId = 1,
                AdminId = 101,
                MinOrderValue = 50m,
                MaxOrderValue = 200m,
                FeeAmount = 10m,
                Status = "Active",
                CreateDate = DateTime.UtcNow
            },
            new PolicySystemDto
            {
                PolicyId = 2,
                AdminId = 102,
                MinOrderValue = 100m,
                MaxOrderValue = 500m,
                FeeAmount = 20m,
                Status = "Inactive",
                CreateDate = DateTime.UtcNow.AddDays(-5)
            }
        };

            _mockPolicySystemService
                .Setup(service => service.GetAllAsync())
                .ReturnsAsync(mockPolicies);

            // Act
            var result = await _controller.GetAllPolicies();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(200, okResult.StatusCode);

            var returnedPolicies = okResult.Value as List<PolicySystemDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedPolicies);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedPolicies.Count);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, returnedPolicies[0].PolicyId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedPolicies[1].PolicyId);
        }
    }
}
