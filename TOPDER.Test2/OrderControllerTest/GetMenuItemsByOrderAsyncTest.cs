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
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetMenuItemsByOrderAsyncTest
    {
        private Mock<IOrderService> _orderServiceMock;
        private Mock<IOrderMenuService> _orderMenuServiceMock;
        private Mock<IOrderTableService> _orderTableServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IDiscountRepository> _discountRepositoryMock;
        private Mock<IMenuRepository> _menuRepositoryMock;
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IRestaurantService> _mockRestaurantService;
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
            _mockRestaurantService = new Mock<IRestaurantService>();
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
                _configurationMock.Object,
                _mockRestaurantService.Object
            );
        }

        [TestMethod]
        public async Task GetMenuItemsByOrderAsync_ShouldReturnNotFound_WhenNoItemsFound()
        {
            // Arrange
            int orderId = 1;
            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync((List<OrderMenuDto>)null); // Không có món

            // Act
            var result = await _controller.GetMenuItemsByOrderAsync(orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy món cho đơn hàng này.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetMenuItemsByOrderAsync_ShouldReturnOk_WhenItemsFound()
        {
            // Arrange
            int orderId = 1;
            var menuItems = new List<OrderMenuDto>
        {
            new OrderMenuDto { MenuId = 1, OrderId = orderId, MenuName = "Menu 1" },
            new OrderMenuDto { MenuId = 2, OrderId = orderId, MenuName = "Menu 2" }
        };

            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync(menuItems); // Mock trả về danh sách món

            // Act
            var result = await _controller.GetMenuItemsByOrderAsync(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as List<OrderMenuDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnValue.Count); // Kiểm tra số lượng món
        }

        [TestMethod]
        public async Task GetMenuItemsByOrderAsync_ShouldReturnNotFound_WhenItemsListIsEmpty()
        {
            // Arrange
            int orderId = 1;
            var emptyMenuList = new List<OrderMenuDto>(); // Danh sách món rỗng

            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync(emptyMenuList); // Mock trả về danh sách món rỗng

            // Act
            var result = await _controller.GetMenuItemsByOrderAsync(orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy món cho đơn hàng này.", notFoundResult.Value);
        }
    }
}
