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
using TOPDER.Service.IServices;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class GetInvisibleTest
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
        public async Task GetInvisible_Success_ReturnsOk()
        {
            // Arrange
            int restaurantId = 1;
            int menuId = 1;
            _menuServiceMock.Setup(service => service.InvisibleAsync(menuId, restaurantId))
                            .ReturnsAsync(true);  // Giả lập thành công

            // Act
            var result = await _controller.GetInvisible(restaurantId, menuId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Xóa/Ẩn Món ăn đã thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task GetInvisible_NotFound_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 1;
            int menuId = 0;
            _menuServiceMock.Setup(service => service.InvisibleAsync(menuId, restaurantId))
                            .ReturnsAsync(false);  // Giả lập không tìm thấy món ăn

            // Act
            var result = await _controller.GetInvisible(restaurantId, menuId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Món ăn không được tìm thấy, không thuộc về nhà hàng đã chỉ định, hoặc đang được sử dụng trong một đơn hàng.", notFoundResult.Value);
        }

       

        [TestMethod]
        public async Task GetInvisible_InvalidRequest_ReturnsNotFound()
        {
            // Arrange
            int restaurantId = 0;  // Invalid restaurantId
            int menuId = 1;  // Valid menuId
            _menuServiceMock.Setup(service => service.InvisibleAsync(menuId, restaurantId))
                            .ReturnsAsync(false);  // Giả lập không tìm thấy món ăn

            // Act
            var result = await _controller.GetInvisible(restaurantId, menuId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Món ăn không được tìm thấy, không thuộc về nhà hàng đã chỉ định, hoặc đang được sử dụng trong một đơn hàng.", notFoundResult.Value);
        }
    }
}
