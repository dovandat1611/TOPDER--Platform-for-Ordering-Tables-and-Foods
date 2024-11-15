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
    public class GetDescriptionAsyncTest
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
        public async Task GetDescription_ReturnsOk_WhenRestaurantExists()
        {
            // Arrange
            var restaurantId = 1; // Một restaurantId hợp lệ, tồn tại trong cơ sở dữ liệu
            var descriptionRestaurant = new DescriptionRestaurant
            {
                RestaurantId = restaurantId,
                Description = "Mô tả nhà hàng",
                Subdescription = "Mô tả phụ"
            };

            // Giả lập service trả về mô tả nhà hàng
            _restaurantServiceMock
                .Setup(s => s.GetDescriptionAsync(restaurantId))
                .ReturnsAsync(descriptionRestaurant);

            // Act
            var result = await _controller.GetDescription(restaurantId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult?.StatusCode);
            var returnedDescription = okResult?.Value as DescriptionRestaurant;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedDescription);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(restaurantId, returnedDescription?.RestaurantId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mô tả nhà hàng", returnedDescription?.Description);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Mô tả phụ", returnedDescription?.Subdescription);
        }
        [TestMethod]
        public async Task GetDescription_ReturnsNotFound_WhenRestaurantDoesNotExist()
        {
            // Arrange
            var restaurantId = 999999; // Một restaurantId không tồn tại trong cơ sở dữ liệu

            // Giả lập service trả về null khi không tìm thấy nhà hàng
            _restaurantServiceMock
                .Setup(s => s.GetDescriptionAsync(restaurantId))
                .ReturnsAsync((DescriptionRestaurant)null);

            // Act
            var result = await _controller.GetDescription(restaurantId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult?.StatusCode);
        }

    }
}
