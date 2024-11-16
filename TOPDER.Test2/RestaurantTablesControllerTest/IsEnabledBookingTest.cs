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
    public class IsEnabledBookingTest
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
        public async Task IsEnabledBooking_Success_ReturnsNoContent()
        {
            // Arrange
            int restaurantId = 1;
            int tableId = 10;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(tableId, restaurantId, isEnabledBooking))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, tableId, isEnabledBooking);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task IsEnabledBooking_NoStateChange_ReturnsBadRequest()
        {
            // Arrange
            int restaurantId = 1;
            int tableId = 10;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(tableId, restaurantId, isEnabledBooking))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, tableId, isEnabledBooking);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Không có sự thay đổi trạng thái bàn.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_TableNotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int tableId = 10;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(tableId, restaurantId, isEnabledBooking))
                        .ThrowsAsync(new KeyNotFoundException("Bàn không tồn tại."));

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, tableId, isEnabledBooking);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Bàn không tồn tại.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task IsEnabledBooking_Unauthorized_ReturnsForbid()
        {
            // Arrange
            int restaurantId = 1;
            int tableId = 10;
            bool isEnabledBooking = true;

            _mockService.Setup(s => s.IsEnabledBookingAsync(tableId, restaurantId, isEnabledBooking))
                        .ThrowsAsync(new UnauthorizedAccessException("Không được phép thay đổi trạng thái bàn."));

            // Act
            var result = await _controller.IsEnabledBooking(restaurantId, tableId, isEnabledBooking);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }
    }
}
