using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class ChangeMenusAsyncTest
    {
        private Mock<IOrderService> _orderServiceMock;
        private Mock<IOrderMenuService> _orderMenuServiceMock;
        private Mock<IOrderTableService> _orderTableServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IDiscountRepository> _discountRepositoryMock;
        private Mock<IMenuRepository> _menuRepositoryMock;
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private Mock<ISendMailService> _sendMailServiceMock;
        private Mock<IDiscountMenuRepository> _discountMenuRepositoryMock;
        private Mock<IConfiguration> _configurationMock;

        private OrderController _controller;

        [TestInitialize]
        public void SetUp()
        {
            // Mock all dependencies
            _orderServiceMock = new Mock<IOrderService>();
            _orderMenuServiceMock = new Mock<IOrderMenuService>();
            _orderTableServiceMock = new Mock<IOrderTableService>();
            _walletServiceMock = new Mock<IWalletService>();
            _discountRepositoryMock = new Mock<IDiscountRepository>();
            _menuRepositoryMock = new Mock<IMenuRepository>();
            _restaurantRepositoryMock = new Mock<IRestaurantRepository>();
            _userServiceMock = new Mock<IUserService>();
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _sendMailServiceMock = new Mock<ISendMailService>();
            _discountMenuRepositoryMock = new Mock<IDiscountMenuRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Create the controller and inject the mocked dependencies
            _controller = new OrderController(
                _orderServiceMock.Object,
                _orderMenuServiceMock.Object,
                _walletServiceMock.Object,
                _menuRepositoryMock.Object,
                _restaurantRepositoryMock.Object,
                _discountRepositoryMock.Object,
                _userServiceMock.Object,
                _walletTransactionServiceMock.Object,
                _paymentGatewayServiceMock.Object,
                _sendMailServiceMock.Object,
                _orderTableServiceMock.Object,
                _discountMenuRepositoryMock.Object,
                _configurationMock.Object
            );
        }

        [TestMethod]
        public async Task ChangeMenusAsync_ShouldReturnBadRequest_WhenMenuListIsEmpty()
        {
            // Arrange
            var changeOrderMenu = new ChangeOrderMenuDto
            {
                RestaurantId = 1,
                CustomerId = 1,
                OrderId = 1,
                orderMenus = new List<OrderMenuModelDto>() // Danh sách món rỗng
            };

            // Act
            var result = await _controller.ChangeMenusAsync(changeOrderMenu);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Menu list cannot be empty.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangeMenusAsync_ShouldReturnBadRequest_WhenNoValidMenusFound()
        {
            // Arrange
            var changeOrderMenu = new ChangeOrderMenuDto
            {
                RestaurantId = 1,
                CustomerId = 1,
                OrderId = 1,
                orderMenus = new List<OrderMenuModelDto>
            {
                new OrderMenuModelDto { MenuId = 99, Quantity = 1 } // Món không hợp lệ
            }
            };

            _menuRepositoryMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Menu)null); // Menu không tồn tại

            // Act
            var result = await _controller.ChangeMenusAsync(changeOrderMenu);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
               Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No valid menus found for the order.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangeMenusAsync_ShouldReturnBadRequest_WhenChangeMenusFails()
        {
            // Arrange
            var changeOrderMenu = new ChangeOrderMenuDto
            {
                RestaurantId = 1,
                CustomerId = 1,
                OrderId = 1,
                orderMenus = new List<OrderMenuModelDto>
            {
                new OrderMenuModelDto { MenuId = 1, Quantity = 2 }
            }
            };

            var menu = new Menu { MenuId = 1, Price = 100m };
            _menuRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(menu);
            _orderMenuServiceMock.Setup(x => x.ChangeMenusAsync(It.IsAny<int>(), It.IsAny<List<CreateOrUpdateOrderMenuDto>>()))
                                 .ReturnsAsync(false); // Thay đổi thực đơn thất bại

            // Act
            var result = await _controller.ChangeMenusAsync(changeOrderMenu);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Failed to change menus.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChangeMenusAsync_ShouldReturnNotFound_WhenRestaurantNotFound()
        {
            // Arrange
            var changeOrderMenu = new ChangeOrderMenuDto
            {
                RestaurantId = 1,
                CustomerId = 1,
                OrderId = 1,
                orderMenus = new List<OrderMenuModelDto>
            {
                new OrderMenuModelDto { MenuId = 1, Quantity = 2 }
            }
            };

            var menu = new Menu { MenuId = 1, Price = 100m };
            _menuRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(menu);
            _orderMenuServiceMock.Setup(x => x.ChangeMenusAsync(It.IsAny<int>(), It.IsAny<List<CreateOrUpdateOrderMenuDto>>()))
                                 .ReturnsAsync(true);

            _restaurantRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Restaurant)null); // Nhà hàng không tồn tại

            // Act
            var result = await _controller.ChangeMenusAsync(changeOrderMenu);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant does not exist.", notFoundResult.Value);
        }
    }
}
