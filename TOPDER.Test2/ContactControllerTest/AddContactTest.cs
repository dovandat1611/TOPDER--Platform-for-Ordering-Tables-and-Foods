using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ContactControllerTest
{
    [TestClass]
    public class AddContactTests
    {
        private Mock<IContactService> _mockContactService;
        private Mock<ISendMailService> _mockSendMailService;
        private ContactController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockContactService = new Mock<IContactService>();
            _mockSendMailService = new Mock<ISendMailService>();
            _controller = new ContactController(_mockContactService.Object, _mockSendMailService.Object);
        }

        [TestMethod]
        public async Task AddContact_WithValidRestaurantRegisterTopic_ReturnsOk()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Topic = Contact_Topic.RESTAURANT_REGISTER,
                Content = "Inquiry about restaurant registration",
                Phone = "123-456-7890"
            };
            _mockContactService.Setup(service => service.AddAsync(contactDto)).ReturnsAsync(true);
            _mockSendMailService
                .Setup(service => service.SendEmailAsync(contactDto.Email, Email_Subject.CONTACT_REGISTER, It.IsAny<string>()))
                .Verifiable();

            // Act
            var result = await _controller.AddContact(contactDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo liên hệ thành công.", result.Value);
            _mockSendMailService.Verify();
        }

        [TestMethod]
        public async Task AddContact_WithValidNonRestaurantTopic_ReturnsOk()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = 2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                Topic = "GENERAL_INQUIRY",
                Content = "General question",
                Phone = "987-654-3210"
            };
            _mockContactService.Setup(service => service.AddAsync(contactDto)).ReturnsAsync(true);
            _mockSendMailService
                .Setup(service => service.SendEmailAsync(contactDto.Email, Email_Subject.CONTACT, It.IsAny<string>()))
                .Verifiable();

            // Act
            var result = await _controller.AddContact(contactDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo liên hệ thành công.", result.Value);
            _mockSendMailService.Verify();
        }

        [TestMethod]
        public async Task AddContact_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var contactDto = new CreateContactDto { Name = "", Email = "invalid-email" }; // Invalid data
            _controller.ModelState.AddModelError("Name", "Name is required"); // Simulate invalid model state
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.AddContact(contactDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // Ensure result contains model state errors
        }

        // Test for null email address
        [TestMethod]
        public async Task AddContact_WithNullEmail_ReturnsBadRequest()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = 1,
                Name = "Test User",
                Email = null, // Null email
                Topic = "GENERAL_INQUIRY",
                Content = "Some inquiry content",
                Phone = "123-456-7890"
            };
            _controller.ModelState.AddModelError("Email", "Email is required"); // Simulate invalid model state

            // Act
            var result = await _controller.AddContact(contactDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // Ensure result contains model state errors
        }

        // Test for successful contact creation with null Uid
        [TestMethod]
        public async Task AddContact_WithNullUid_ReturnsOk()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = null, // Null Uid
                Name = "John Doe",
                Email = "john@example.com",
                Topic = "GENERAL_INQUIRY",
                Content = "General inquiry content",
                Phone = "123-456-7890"
            };
            _mockContactService.Setup(service => service.AddAsync(contactDto)).ReturnsAsync(true);
            _mockSendMailService
                .Setup(service => service.SendEmailAsync(contactDto.Email, Email_Subject.CONTACT, It.IsAny<string>()))
                .Verifiable();

            // Act
            var result = await _controller.AddContact(contactDto) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo liên hệ thành công.", result.Value);
            _mockSendMailService.Verify();
        }

        [TestMethod]
        public async Task AddContact_WithMissingPhone_ReturnsBadRequest()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = 1,
                Name = "Customer Name",
                Email = "customer@example.com",
                Topic = "GENERAL_INQUIRY",
                Content = "Some content",
                Phone = null // Missing required Phone
            };
            _controller.ModelState.AddModelError("Phone", "Phone is required");

            // Act
            var result = await _controller.AddContact(contactDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // Check that model state errors are returned
        }

        // Test for invalid model with missing required fields
        [TestMethod]
        public async Task AddContact_WithMissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = 1,
                Name = null, // Missing required Name
                Email = "user@example.com",
                Topic = "GENERAL_INQUIRY",
                Content = null, // Missing required Content
                Phone = "123-456-7890"
            };
            _controller.ModelState.AddModelError("Name", "Name is required");
            _controller.ModelState.AddModelError("Content", "Content is required");

            // Act
            var result = await _controller.AddContact(contactDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Value, typeof(SerializableError)); // Check that model state errors are returned
        }


        [TestMethod]
        public async Task AddContact_WithInvalidUid_ReturnsBadRequest()
        {
            // Arrange
            var contactDto = new CreateContactDto
            {
                Uid = -1, // Invalid Uid
                Name = "Customer Name",
                Email = "customer@example.com",
                Topic = "GENERAL_INQUIRY",
                Content = "Some content",
                Phone = "123-456-7890"
            };

            _mockContactService.Setup(service => service.AddAsync(contactDto)).ReturnsAsync(false); // Simulate failure in contact creation

            // Act
            var result = await _controller.AddContact(contactDto) as BadRequestObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm liên hệ thất bại.", result.Value); // Assert that the failure message is returned
        }
    }
}
