using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class GetDiscountAndFeeTest
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
        public async Task GetDiscountAndFee_ReturnsOk_WhenRestaurantIdExists()
        {
            // Arrange: restaurantId hợp lệ, giả lập dữ liệu trả về từ service
            var restaurantId = 1;
            var discountAndFee = new DiscountAndFeeRestaurant
            {
                RestaurantId = restaurantId,
                DiscountRestaurant = 10m,
                FirstFeePercent = 5m,
                ReturningFeePercent = 10m,
                CancellationFeePercent = 15m
            };

            // Giả lập service trả về discountAndFee
            _restaurantServiceMock
                .Setup(s => s.GetDiscountAndFeeAsync(restaurantId))
                .ReturnsAsync(discountAndFee);

            // Act
            var result = await _controller.GetDiscountAndFee(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
            var responseValue = okResult?.Value as DiscountAndFeeRestaurant;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(responseValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(restaurantId, responseValue?.RestaurantId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10m, responseValue?.DiscountRestaurant);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5m, responseValue?.FirstFeePercent);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10m, responseValue?.ReturningFeePercent);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(15m, responseValue?.CancellationFeePercent);
        }

        [TestMethod]
        public async Task GetDiscountAndFee_ReturnsNotFound_WhenRestaurantIdDoesNotExist()
        {
            // Arrange: restaurantId không tồn tại
            var restaurantId = 999999;  // ID không tồn tại

            // Giả lập service trả về null (không tìm thấy nhà hàng)
            _restaurantServiceMock
                .Setup(s => s.GetDiscountAndFeeAsync(restaurantId))
                .ReturnsAsync((DiscountAndFeeRestaurant)null);

            // Act
            var result = await _controller.GetDiscountAndFee(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult?.StatusCode);
        }


    }
}
