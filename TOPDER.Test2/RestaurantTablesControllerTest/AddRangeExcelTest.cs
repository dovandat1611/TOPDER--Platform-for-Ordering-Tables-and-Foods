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
        public async Task AddRangeExcel_NullFile_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateExcelRestaurantTableDto
            {
                File = null
            };

            // Act
            var result = await _controller.AddRangeExcel(dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có tệp nào được tải lên.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddRangeExcel_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var emptyFileMock = new Mock<IFormFile>();
            emptyFileMock.Setup(f => f.Length).Returns(0);

            var dto = new CreateExcelRestaurantTableDto
            {
                File = emptyFileMock.Object
            };

            // Act
            var result = await _controller.AddRangeExcel(dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không có tệp nào được tải lên.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddRangeExcel_Success_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024); // File has content

            var dto = new CreateExcelRestaurantTableDto
            {
                File = fileMock.Object
            };

            _mockService.Setup(s => s.AddRangeExcelAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddRangeExcel(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo danh sách bàn thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task AddRangeExcel_Failure_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024); // File has content

            var dto = new CreateExcelRestaurantTableDto
            {
                File = fileMock.Object
            };

            _mockService.Setup(s => s.AddRangeExcelAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddRangeExcel(dto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không thể tạo danh sách bàn từ Excel.", badRequestResult.Value);
        }
    }
}
