using Microsoft.AspNetCore.Mvc;
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
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class PaidOrderTest
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
        public async Task PaidOrder_OrderNotFound_ReturnsNotFound()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var paymentGateway = "ISBALANCE";

            _orderServiceMock
                .Setup(service => service.GetItemAsync(orderId, userId))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task PaidOrder_UnauthorizedAccess_ReturnsForbidden()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var paymentGateway = "ISBALANCE";

            _orderServiceMock
                .Setup(service => service.GetItemAsync(orderId, userId))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(403, 403);
        }


        [TestMethod]
        public async Task PaidOrder_ReturnsOk_WhenWalletBalanceIsSufficient()
        {
            // Arrange
            var orderId = 1;
            var userId = 101;
            var paymentGateway = PaymentGateway.ISBALANCE;
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                CustomerId = userId,
                TotalAmount = 100,
                StatusPayment = Payment_Status.PENDING
            };

            _orderServiceMock
                .Setup(service => service.GetItemAsync(orderId, userId))
                .ReturnsAsync(orderDto);

            // Mock UpdatePaidOrderAsync to return true
            _orderServiceMock
                .Setup(service => service.UpdatePaidOrderAsync(It.IsAny<OrderDto>()))
                .ReturnsAsync(true);

            _walletServiceMock
                .Setup(service => service.GetBalanceOrderAsync(userId))
                .ReturnsAsync(200); // Sufficient balance

            _userServiceMock
                .Setup(service => service.GetInformationUserOrderIsBalance(userId))
                .ReturnsAsync(new UserOrderIsBalance { WalletId = 1, Id = userId });

            _walletTransactionServiceMock
                .Setup(service => service.AddAsync(It.IsAny<WalletTransactionDto>()))
                .ReturnsAsync(true);

            _walletServiceMock
                .Setup(service => service.UpdateWalletBalanceOrderAsync(It.IsAny<WalletBalanceOrderDto>()))
                .ReturnsAsync(true);

            _orderServiceMock
                .Setup(service => service.UpdateStatusAsync(orderId, Order_Status.PAID))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thanh toán đơn hàng thành công", okResult.Value);
        }

        [TestMethod]
        public async Task PaidOrder_ReturnsUrl_WhenPaymentGatewayIsVIETQR()
        {
            // Arrange
            var orderId = 2;
            var userId = 102;
            var paymentGateway = PaymentGateway.VIETQR;

            // Mocked order data
            var orderDto = new OrderDto
            {
                OrderId = orderId,
                CustomerId = userId,
                TotalAmount = 150,
                StatusPayment = Payment_Status.PENDING
            };

            // Mocked order menu data
            var orderMenuDtos = new List<OrderMenuDto>
            {
                new OrderMenuDto { MenuId = 1, Quantity = 2 },
                new OrderMenuDto { MenuId = 2, Quantity = 3 }
            };

            // Expected payment result
            var expectedPaymentResult = new CreatePaymentResult(
                bin: "123456",
                accountNumber: "9876543210",
                amount: 150,
                description: "Payment for Order #2",
                orderCode: 112233445566,
                currency: "VND",
                paymentLinkId: "VIETQR123",
                status: "PENDING",
                checkoutUrl: "https://vietqr.com/payment",
                qrCode: "https://vietqr.com/qrcode"
            );

            // Mock service behavior
            _orderServiceMock
                .Setup(service => service.GetItemAsync(orderId, userId))
                .ReturnsAsync(orderDto);

            _orderServiceMock
                .Setup(service => service.UpdatePaidOrderAsync(It.IsAny<OrderDto>()))
                .ReturnsAsync(true);

            _orderMenuServiceMock
                .Setup(service => service.GetItemsByOrderAsync(orderId))
                .ReturnsAsync(orderMenuDtos);

            _menuRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Menu { DishName = "Dish " + id, Price = 50 });

            _paymentGatewayServiceMock
                .Setup(service => service.CreatePaymentUrlPayOS(It.IsAny<PaymentData>()))
                .ReturnsAsync(expectedPaymentResult);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("https://vietqr.com/payment", okResult.Value);
        }


        [TestMethod]
        public async Task PaidOrder_InvalidPaymentGateway_ReturnsBadRequest()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var paymentGateway = "INVALID_GATEWAY";
            var order = new OrderDto { OrderId = orderId, CustomerId = userId, TotalAmount = 100 };

            _orderServiceMock
                .Setup(service => service.GetItemAsync(orderId, userId))
                .ReturnsAsync(order);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}


