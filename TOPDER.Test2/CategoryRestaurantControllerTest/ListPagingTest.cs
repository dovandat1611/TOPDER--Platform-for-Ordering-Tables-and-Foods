using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.CategoryRestaurantControllerTest
{
    [TestClass]
    public class ListPagingTest
    {
        private Mock<ICategoryRestaurantService> _mockCategoryRestaurantService;
        private CategoryRestaurantController _controller;

        [TestInitialize]
        public void Initialize()
        {
            // Mock dependencies
            _mockCategoryRestaurantService = new Mock<ICategoryRestaurantService>();
            // Create instance of the controller with mocked dependencies
            _controller = new CategoryRestaurantController(_mockCategoryRestaurantService.Object);
        }

        [TestMethod]
        public async Task ListPaging_ValidParameters_ReturnsOk()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var categoryRestaurantName = "Italian";
            List<CategoryRestaurantViewDto> categoryList = new List<CategoryRestaurantViewDto>
            {
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 1,
                    CategoryRestaurantName = "Italian",
                    IsDelete = false
                }
            };

            var mockResult = new PaginatedList<CategoryRestaurantViewDto>(
                categoryList,    // List of items
                pageNumber,                // Page index
                1,                         // Total pages
                categoryList.Count // Total count (number of items)
            );

            _mockCategoryRestaurantService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, categoryRestaurantName))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.ListPaging(pageNumber, pageSize, categoryRestaurantName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<CategoryRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Italian", response.Items[0].CategoryRestaurantName);
        }

        [TestMethod]
        public async Task ListPaging_categoryRestaurantNameIsEmpty_ReturnsOk()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var categoryRestaurantName = "";
            List<CategoryRestaurantViewDto> categoryList = new List<CategoryRestaurantViewDto>
            {
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 1,
                    CategoryRestaurantName = "Italian",
                    IsDelete = false
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 2,
                    CategoryRestaurantName = "Japanese",
                    IsDelete = false
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 3,
                    CategoryRestaurantName = "Mexican",
                    IsDelete = true
                },
                new CategoryRestaurantViewDto
                {
                    CategoryRestaurantId = 4,
                    CategoryRestaurantName = "Chinese",
                    IsDelete = false
                }
            };

            var mockResult = new PaginatedList<CategoryRestaurantViewDto>(
                categoryList,    // List of items
                pageNumber,                // Page index
                1,                         // Total pages
                categoryList.Count // Total count (number of items)
            );

            _mockCategoryRestaurantService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, categoryRestaurantName))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.ListPaging(pageNumber, pageSize, categoryRestaurantName);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<CategoryRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.PageIndex);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.TotalPages);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
        }

        [TestMethod]
        public async Task ListPaging_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var categoryRestaurantName = "Italian";

            _mockCategoryRestaurantService.Setup(service => service.ListPagingAsync(pageNumber, pageSize, categoryRestaurantName))
                .ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act
            var result = await _controller.ListPaging(pageNumber, pageSize, categoryRestaurantName);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(internalServerErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, internalServerErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while processing your request: An unexpected error occurred", internalServerErrorResult.Value);
        }
    }
}
