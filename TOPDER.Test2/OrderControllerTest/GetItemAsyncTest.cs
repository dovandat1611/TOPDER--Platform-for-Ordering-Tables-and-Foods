using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Utils;
using Microsoft.Extensions.Configuration;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using Microsoft.AspNetCore.SignalR;
using TOPDER.Service.Hubs;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetItemAsyncTest
    {
        private Mock<IOrderService> _orderServiceMock;
        private Mock<IOrderMenuService> _orderMenuServiceMock;
        private Mock<IOrderTableService> _orderTableServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IDiscountRepository> _discountRepositoryMock;
        private Mock<IMenuRepository> _menuRepositoryMock;
        private Mock<IRestaurantRepository> _restaurantRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private Mock<ISendMailService> _sendMailServiceMock;
        private Mock<IDiscountMenuRepository> _discountMenuRepositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IHubContext<AppHub>> _signalRHubMock;
        private Mock<IRestaurantPolicyService> _restaurantPolicyServiceMock;
        private Mock<IOrderRepository> _orderRepositoryMock;

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
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _sendMailServiceMock = new Mock<ISendMailService>();
            _discountMenuRepositoryMock = new Mock<IDiscountMenuRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _notificationServiceMock = new Mock<INotificationService>();
            _signalRHubMock = new Mock<IHubContext<AppHub>>();
            _restaurantPolicyServiceMock = new Mock<IRestaurantPolicyService>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

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
                _restaurantServiceMock.Object,
                _notificationServiceMock.Object,
                _signalRHubMock.Object,
                _restaurantPolicyServiceMock.Object,
                _orderRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnOk_WithOrderDetails()
        {
            // Arrange
            int orderId = 1;
            int Uid = 2;

            var orderDto = new OrderDto
            {
                OrderId = orderId,
                CustomerId = Uid
            };
            var orderTables = new List<OrderTableDto>
        {
            new OrderTableDto { TableId = 1, TableName = "Table 1" }
        };
            var orderMenus = new List<OrderMenuDto>
        {
            new OrderMenuDto { MenuId = 1, MenuName = "Pizza", Quantity = 2 }
        };

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, Uid)).ReturnsAsync(orderDto);
            _orderTableServiceMock.Setup(s => s.GetItemsByOrderAsync(orderId)).ReturnsAsync(orderTables);
            _orderMenuServiceMock.Setup(s => s.GetItemsByOrderAsync(orderId)).ReturnsAsync(orderMenus);

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(OrderDto));
            var returnedOrder = okResult.Value as OrderDto;

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderDto.OrderId, returnedOrder.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderTables.Count, returnedOrder.OrderTables.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderMenus.Count, returnedOrder.OrderMenus.Count);

            _orderServiceMock.Verify(s => s.GetItemAsync(orderId, Uid), Times.Once);
            _orderTableServiceMock.Verify(s => s.GetItemsByOrderAsync(orderId), Times.Once);
            _orderMenuServiceMock.Verify(s => s.GetItemsByOrderAsync(orderId), Times.Once);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;
            int Uid = 2;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, Uid)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Đơn hàng với ID {orderId} không tồn tại.", notFoundResult.Value);

            _orderServiceMock.Verify(s => s.GetItemAsync(orderId, Uid), Times.Once);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnForbid_WhenUserNotAuthorized()
        {
            // Arrange
            int orderId = 1;
            int Uid = 2;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, Uid)).ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ForbidResult));
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);

            _orderServiceMock.Verify(s => s.GetItemAsync(orderId, Uid), Times.Once);
        }

    }
}
