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

namespace TOPDER.Test2.RestaurantTablesControllerTest
{
    [TestClass]
    public class SetInvisibleTest
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
        public async Task SetInvisible_Success_ReturnsOk()
        {
            // Arrange
            int restaurantId = 10;
            int tableId = 1;
            _mockService.Setup(s => s.InvisibleAsync(tableId, restaurantId)).ReturnsAsync(true);

            // Act
            var result = await _controller.SetInvisible(restaurantId, tableId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Ẩn/Xóa Table thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task SetInvisible_TableNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 10;
            int tableId = -1;
            _mockService.Setup(s => s.InvisibleAsync(tableId, restaurantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, tableId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy bàn với ID {tableId} hoặc không thuộc nhà hàng đã chỉ định.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task SetInvisible_Unauthorized_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 5; // Not matching the table's restaurant ID
            int tableId = 1;
            _mockService.Setup(s => s.InvisibleAsync(tableId, restaurantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.SetInvisible(restaurantId, tableId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Không tìm thấy bàn với ID {tableId} hoặc không thuộc nhà hàng đã chỉ định.", notFoundResult.Value);
        }

    }
}
