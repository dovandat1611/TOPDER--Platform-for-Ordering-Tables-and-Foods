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

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class UpdateDiscountAndFeeTest
    {
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private RestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new RestaurantController(_restaurantServiceMock.Object, _cloudinaryServiceMock.Object); // Inject mock service
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange: Tham số hợp lệ
            var restaurantId = 1;
            var discountPrice = 10m;
            var firstFeePercent = 5m;
            var returningFeePercent = 5m;
            var cancellationFeePercent = 10m;

            // Giả lập service trả về true (thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenNoChanges()
        {
            // Arrange: Tham số null, không có thay đổi
            var restaurantId = 1;
            decimal? discountPrice = null;
            decimal? firstFeePercent = null;
            decimal? returningFeePercent = null;
            decimal? cancellationFeePercent = null;

            // Giả lập service trả về false (không có thay đổi)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenServiceReturnsFalse()
        {
            // Arrange: Tham số hợp lệ nhưng service trả về false
            var restaurantId = 1;
            var discountPrice = 20m;
            var firstFeePercent = 10m;
            var returningFeePercent = 15m;
            var cancellationFeePercent = 25m;

            // Giả lập service trả về false (không thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenDiscountPriceIsNull()
        {
            // Arrange: discountPrice là null
            var restaurantId = 1;
            decimal? discountPrice = null;
            var firstFeePercent = 10m;
            var returningFeePercent = 5m;
            var cancellationFeePercent = 15m;

            // Giả lập service trả về true (thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenFirstFeePercentIsNull()
        {
            // Arrange: firstFeePercent là null
            var restaurantId = 1;
            var discountPrice = 10m;
            decimal? firstFeePercent = null;
            var returningFeePercent = 5m;
            var cancellationFeePercent = 15m;

            // Giả lập service trả về true (thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
            
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenReturningFeePercentIsNull()
        {
            // Arrange: returningFeePercent là null
            var restaurantId = 1;
            var discountPrice = 10m;
            var firstFeePercent = 10m;
            decimal? returningFeePercent = null;
            var cancellationFeePercent = 15m;

            // Giả lập service trả về true (thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
        }

        [TestMethod]
        public async Task UpdateDiscountAndFee_ReturnsBadRequest_WhenCancellationFeePercentIsNull()
        {
            // Arrange: cancellationFeePercent là null
            var restaurantId = 1;
            var discountPrice = 10m;
            var firstFeePercent = 10m;
            var returningFeePercent = 5m;
            decimal? cancellationFeePercent = null;

            // Giả lập service trả về true (thành công)
            _restaurantServiceMock
                .Setup(s => s.UpdateDiscountAndFeeAsync(restaurantId, discountPrice, firstFeePercent, returningFeePercent, cancellationFeePercent))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDiscountAndFee(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
        }
    }
}
