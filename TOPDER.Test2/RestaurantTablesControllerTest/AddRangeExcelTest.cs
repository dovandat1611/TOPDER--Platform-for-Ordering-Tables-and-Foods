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
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantTablesControllerTest
{
    [TestClass]
    public class AddRangeExcelTest
    {
        private Mock<IRestaurantTableService> _mockService;
        private RestaurantTablesController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IRestaurantTableService>();
            _controller = new RestaurantTablesController(_mockService.Object);
        }

        [TestMethod]
        public async Task AddRangeFromExcel_ShouldReturnOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var createExcelRestaurantTableDto = new CreateExcelRestaurantTableDto
            {
                // Initialize properties as needed
                RestaurantId = 1,
                File = new FormFile(new MemoryStream(), 0, 0, "Data", "test.xlsx") // Simulated file
            };

            _mockService
                .Setup(s => s.AddRangeExcelAsync(createExcelRestaurantTableDto))
                .ReturnsAsync((true, "Tables added successfully"));

            // Act
            var result = await _controller.AddRangeFromExcel(createExcelRestaurantTableDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tables added successfully", okResult.Value);

        }

        [TestMethod]
        public async Task AddRangeFromExcel_ShouldReturnServerError_WhenServiceReturnsFailure()
        {
            // Arrange
            var createExcelRestaurantTableDto = new CreateExcelRestaurantTableDto
            {
                // Initialize properties as needed
                RestaurantId = 1,
                File = new FormFile(new MemoryStream(), 0, 0, "Data", "test.xlsx") // Simulated file
            };

            _mockService
                .Setup(s => s.AddRangeExcelAsync(createExcelRestaurantTableDto))
                .ReturnsAsync((false, "Error adding tables"));

            // Act
            var result = await _controller.AddRangeFromExcel(createExcelRestaurantTableDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Error adding tables", objectResult.Value);

        }

    }
}
