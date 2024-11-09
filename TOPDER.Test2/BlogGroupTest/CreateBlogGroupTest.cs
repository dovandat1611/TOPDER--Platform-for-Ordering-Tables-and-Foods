using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BlogGroupTest
{
    [TestClass]
    public class CreateBlogGroupTest
    {
        private Mock<IBlogGroupService> mockBlogGroupService;
        private BlogGroupController controller;

        [TestInitialize]
        public void Initialize()
        {
            mockBlogGroupService = new Mock<IBlogGroupService>();
            controller = new BlogGroupController(mockBlogGroupService.Object);
        }
        [TestMethod]
        public async Task AddAsync_WhenBlogGroupIsValid_ReturnsOk()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto
            {
                BloggroupId = 1,
                BloggroupName = "Technology"
            };

            // Mock the service call to return true (success)
            mockBlogGroupService.Setup(service => service.AddAsync(It.IsAny<BlogGroupDto>())).ReturnsAsync(true);

            // Act
            var result = await controller.CreateBlogGroup(blogGroupDto);  // Ensure this matches the correct method in the controller

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);  // Ensure the result is OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);  // Ensure the status code is 200 (OK)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo Blog Group thành công.", okResult.Value);  // Ensure the success message is returned
        }

        [TestMethod]
        public async Task AddAsync_WhenModelStateIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var blogGroupDto = new BlogGroupDto
            {
                BloggroupId = 0,  // Invalid BloggroupId to trigger model validation failure
                BloggroupName = "Invalid BlogGroup"
            };

            // Add a model error to simulate invalid ModelState
            controller.ModelState.AddModelError("BloggroupId", "BloggroupId is required");

            // Act
            var result = await controller.CreateBlogGroup(blogGroupDto);  // Ensure this matches the correct method in the controller

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Ensure the result is BadRequestObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);  // Ensure the status code is 400 (Bad Request)
        }
    }
}
