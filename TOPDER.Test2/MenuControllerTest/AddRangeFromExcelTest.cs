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
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class AddRangeFromExcelTest
    {
        private Mock<IMenuService> _menuServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private MenuController _controller;

        [TestInitialize]
        public void Setup()
        {
            _menuServiceMock = new Mock<IMenuService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new MenuController(_menuServiceMock.Object, _cloudinaryServiceMock.Object);
        }

        [TestMethod]
        public async Task AddRangeFromExcel_Success_ReturnsOk()
        {
            // Arrange
            var createExcelMenuDto = new CreateExcelMenuDto
            {
                RestaurantId = 1,
                File = new Mock<IFormFile>().Object  // Mock the IFormFile object
            };

            // Mock the service method to return true, indicating success
            _menuServiceMock.Setup(service => service.AddRangeExcelAsync(It.IsAny<CreateExcelMenuDto>()))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.AddRangeFromExcel(createExcelMenuDto);

            // Assert
            var okResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Menu items added successfully from Excel.", okResult.Value);
        }

        [TestMethod]
        public async Task AddRangeFromExcel_Failed_Returns500()
        {
            // Arrange
            var createExcelMenuDto = new CreateExcelMenuDto
            {
                RestaurantId = 1,
                File = new Mock<IFormFile>().Object  // Mock the IFormFile object
            };

            // Mock the service method to return false, indicating failure
            _menuServiceMock.Setup(service => service.AddRangeExcelAsync(It.IsAny<CreateExcelMenuDto>()))
                            .ReturnsAsync(false);

            // Act
            var result = await _controller.AddRangeFromExcel(createExcelMenuDto);

            // Assert
            var objectResult = result as ObjectResult;  // Cast result to ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(objectResult);  // Ensure the result is an ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);  // Check the status code is 500
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to add menu items from Excel.", objectResult.Value);  // Ensure the response message is correct
        }

        
    }
}
