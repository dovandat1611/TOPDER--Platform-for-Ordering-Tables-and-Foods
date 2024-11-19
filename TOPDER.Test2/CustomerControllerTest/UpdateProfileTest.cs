using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.CustomerControllerTest
{
    [TestClass]
    public class UpdateProfileTest
    {
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<ICloudinaryService> _mockCloudinaryService;
        private CustomerController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _controller = new CustomerController(_mockCustomerService.Object, _mockCloudinaryService.Object);
        }

        // Test case for invalid model state
        [TestMethod]
        public async Task UpdateProfile_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var customerProfile = new CustomerProfileDto { Uid = 1 };

            // Act
            var result = await _controller.UpdateProfile(customerProfile) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
        }

        // Test case for valid profile without image
        [TestMethod]
        public async Task UpdateProfile_ValidProfileWithoutImage_ReturnsOk()
        {
            // Arrange
            var customerProfile = new CustomerProfileDto
            {
                Uid = 1,
                Name = "John Doe",
                Phone = "1234567890",
                Gender = "Nam"
            };

            _mockCustomerService.Setup(service => service.UpdateProfile(customerProfile)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(customerProfile) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.                   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        // Test case for valid profile with image
        [TestMethod]
        public async Task UpdateProfile_ValidProfileWithImage_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var customerProfile = new CustomerProfileDto
            {
                Uid = 1,
                Name = "John Doe",
                Phone = "1234567890",
                ImageFile = fileMock.Object
            };

            var uploadResult = new ImageUploadResult { SecureUrl = new Uri("http://imageurl.com") };
            _mockCloudinaryService.Setup(service => service.UploadImageAsync(fileMock.Object)).ReturnsAsync(uploadResult);
            _mockCustomerService.Setup(service => service.UpdateProfile(customerProfile)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(customerProfile) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        

        // Test case for successful profile update
        [TestMethod]
        public async Task UpdateProfile_SuccessfulUpdate_ReturnsOk()
        {
            // Arrange
            var customerProfile = new CustomerProfileDto
            {
                Uid = 1,
                Name = "John Doe",
                Phone = "1234567890"
            };

            _mockCustomerService.Setup(service => service.UpdateProfile(customerProfile)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(customerProfile) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
        }

        
    }
}
