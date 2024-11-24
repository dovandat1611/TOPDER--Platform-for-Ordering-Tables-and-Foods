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
    public class GetRelateRestaurantByCategoryAsyncTest
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
        public async Task GetRelatedRestaurants_ReturnsOk_WhenRestaurantsFound()
        {
            // Arrange: Tạo dữ liệu giả cho nhà hàng liên quan
            var restaurantId = 1;
            var categoryRestaurantId = 9999;
            var relatedRestaurants = new List<RestaurantDto>
    {
        new RestaurantDto { Uid = 2, NameRes = "Restaurant 1", CategoryRestaurantId = categoryRestaurantId },
        new RestaurantDto { Uid = 3, NameRes = "Restaurant 2", CategoryRestaurantId = categoryRestaurantId }
    };

            // Giả lập service trả về danh sách nhà hàng liên quan
            _restaurantServiceMock
                .Setup(s => s.GetRelateRestaurantByCategoryAsync(restaurantId, categoryRestaurantId))
                .ReturnsAsync(relatedRestaurants);

            // Act
            var result = await _controller.GetRelatedRestaurants(restaurantId, categoryRestaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult?.StatusCode);
            var responseValue = okResult?.Value as List<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(responseValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, responseValue.Count); // Kiểm tra có 2 nhà hàng liên quan
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", responseValue[0].NameRes);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 2", responseValue[1].NameRes);
        }

        [TestMethod]
        public async Task GetRelatedRestaurants_ReturnsOk_WhenNoRelatedRestaurantsFound()
        {
            // Arrange: Tạo dữ liệu giả cho trường hợp không có nhà hàng liên quan
            var restaurantId = 9999;
            var categoryRestaurantId = 1;

            // Giả lập service trả về danh sách trống (không có nhà hàng liên quan)
            _restaurantServiceMock
                .Setup(s => s.GetRelateRestaurantByCategoryAsync(restaurantId, categoryRestaurantId))
                .ReturnsAsync(new List<RestaurantDto>());

            // Act
            var result = await _controller.GetRelatedRestaurants(restaurantId, categoryRestaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
            var responseValue = okResult?.Value as List<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(responseValue);
                   Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, responseValue.Count); // Kiểm tra không có nhà hàng liên quan
        }

        [TestMethod]
        public async Task GetRelatedRestaurants_ReturnsStatusCode500_WhenExceptionOccurs()
        {
            // Arrange: Tạo dữ liệu giả, nhưng giả lập exception xảy ra trong service
            var restaurantId = 1;
            var categoryRestaurantId = 1;

            // Giả lập service ném exception
            _restaurantServiceMock
                .Setup(s => s.GetRelateRestaurantByCategoryAsync(restaurantId, categoryRestaurantId))
                .ThrowsAsync(new Exception("Lỗi hệ thống"));

            // Act
            var result = await _controller.GetRelatedRestaurants(restaurantId, categoryRestaurantId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult?.StatusCode);
        }

    }
}
