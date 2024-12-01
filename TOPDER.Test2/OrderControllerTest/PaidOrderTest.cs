using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class OrderControllerTests
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
        public async Task PaidOrder_ValidISBALANCEPayment_ReturnsOk()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var paymentGateway = PaymentGateway.ISBALANCE;
            var typeOrder = Paid_Type.ENTIRE_ORDER;

            var mockOrder = new OrderDto
            {
                OrderId = orderId,
                CustomerId = userId,
                RestaurantId = 1,
                TotalAmount = 100,
                StatusPayment = Payment_Status.PENDING
            };

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                .ReturnsAsync(mockOrder);

            _walletServiceMock.Setup(s => s.GetBalanceOrderAsync(userId))
                .ReturnsAsync(200);

            _walletServiceMock.Setup(s => s.UpdateWalletBalanceOrderAsync(It.IsAny<WalletBalanceOrderDto>()))
                .ReturnsAsync(true);

            _orderServiceMock.Setup(s => s.UpdateStatusAsync(orderId, Order_Status.PAID))
                .ReturnsAsync(mockOrder);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thanh toán đơn hàng thành công", okResult.Value);
        }

        [TestMethod]
        public async Task PaidOrder_InvalidTypeOrder_ReturnsBadRequest()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var paymentGateway = PaymentGateway.ISBALANCE;
            var invalidTypeOrder = "INVALID";

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, invalidTypeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Chọn typeOrder là Deposit hoặc EntireOrder!", ((dynamic)badRequestResult.Value).message);
        }

        [TestMethod]
        public async Task PaidOrder_InvalidPaymentGateway_ReturnsBadRequest()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var invalidPaymentGateway = "INVALID";
            var typeOrder = Paid_Type.ENTIRE_ORDER;

            var mockOrder = new OrderDto
            {
                OrderId = orderId,
                CustomerId = userId,
                RestaurantId = 1,
                TotalAmount = 100,
                StatusPayment = Payment_Status.PENDING
            };

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                .ReturnsAsync(mockOrder);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, invalidPaymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cổng thanh toán không hợp lệ.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PaidOrder_OrderNotFound_ReturnsNotFound()
        {
            // Arrange
            var orderId = 9999;
            var userId = 1;
            var paymentGateway = PaymentGateway.ISBALANCE;
            var typeOrder = Paid_Type.ENTIRE_ORDER;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Order not found", ((dynamic)notFoundResult.Value).message);
        }
    }

}


