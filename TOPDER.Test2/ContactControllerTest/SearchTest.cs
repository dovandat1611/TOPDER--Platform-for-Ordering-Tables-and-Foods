using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.IServices;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.ContactControllerTest
{
    [TestClass]
    public class SearchTest
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

        // Test case for valid search with content and topic filters
        [TestMethod]
        public async Task Search_WithContentAndTopic_ReturnsPaginatedResponse()
        {
            // Arrange
            string content = "Sample Content";
            string topic = "Sample Topic";
            int pageNumber = 1;
            int pageSize = 10;

            var contactList = new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Topic = topic,
                    Content = content,
                    Phone = "1234567890"
                }
            };

            var paginatedResult = new PaginatedList<ContactDto>(contactList, 1, 1, 10);

            _mockContactService.Setup(service => service.SearchPagingAsync(pageNumber, pageSize, content, topic))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.Search(content, topic, pageNumber, pageSize) as OkObjectResult;

            // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as PaginatedResponseDto<ContactDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
                Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.AreEqual("John Doe", response.Items[0].Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(content, response.Items[0].Content);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(topic, response.Items[0].Topic);
        }

        // Test case for search with no filters, using pagination
        [TestMethod]
        public async Task Search_WithoutFilters_ReturnsPaginatedResponse()
        {
            // Arrange
            string content = null;
            string topic = null;
            int pageNumber = 1;
            int pageSize = 10;

            var paginatedResult = new PaginatedList<ContactDto>(new List<ContactDto>
            {
                new ContactDto
                {
                    ContactId = 1,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Topic = "General Inquiry",
                    Content = "Inquiry about the product",
                    Phone = "0987654321"
                }
            }, 1, 1, 10);

            _mockContactService.Setup(service => service.SearchPagingAsync(pageNumber, pageSize, content, topic))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.Search(content, topic, pageNumber, pageSize) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as PaginatedResponseDto<ContactDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Jane Smith", response.Items[0].Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Inquiry about the product", response.Items[0].Content);
        }

        // Test case for search with empty result
        [TestMethod]
        public async Task Search_WithNoMatchingContacts_ReturnsEmptyList()
        {
            // Arrange
            string content = "Non-existent Content";
            string topic = "Non-existent Topic";
            int pageNumber = 1;
            int pageSize = 10;

            var paginatedResult = new PaginatedList<ContactDto>(new List<ContactDto>(), 1, 0, 10);

            _mockContactService.Setup(service => service.SearchPagingAsync(pageNumber, pageSize, content, topic))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.Search(content, topic, pageNumber, pageSize) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as PaginatedResponseDto<ContactDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count);
        }
    }
}
