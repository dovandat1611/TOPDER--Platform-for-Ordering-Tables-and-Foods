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
using TOPDER.Service.Dtos.RestaurantPolicy;
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
        public async Task PaidOrder_OrderNotFound_ReturnsNotFound()
        {
            // Arrange
            int orderId = 1;
            int userId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            string typeOrder = Paid_Type.ENTIRE_ORDER;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                             .Throws(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var objectResult = (NotFoundObjectResult)result;
        }

        [TestMethod]
        public async Task PaidOrder_UnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            int orderId = 1;
            int userId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            string typeOrder = Paid_Type.ENTIRE_ORDER;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                             .Throws(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task PaidOrder_InvalidTypeOrder_ReturnsBadRequest()
        {
            // Arrange
            int orderId = 1;
            int userId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            string typeOrder = "InvalidType";

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var objectResult = (BadRequestObjectResult)result;
        }

        [TestMethod]
        public async Task PaidOrder_InsufficientWalletBalance_ReturnsBadRequest()
        {
            // Arrange
            int orderId = 1;
            int userId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            string typeOrder = Paid_Type.ENTIRE_ORDER;
            decimal totalAmount = 100;
            decimal walletBalance = 50;

            _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId)).ReturnsAsync(new OrderDto
            {
                OrderId = orderId,
                CustomerId = userId,
                TotalAmount = totalAmount
            });

            _walletServiceMock.Setup(w => w.GetBalanceOrderAsync(userId)).ReturnsAsync(walletBalance);

            // Act
            var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var objectResult = (BadRequestObjectResult)result;
        }

        [TestMethod]
public async Task PaidOrder_SuccessfulWalletPayment_ReturnsOk()
{
    // Arrange
    int orderId = 1;
    int userId = 1;
    string paymentGateway = PaymentGateway.ISBALANCE;
    string typeOrder = Paid_Type.ENTIRE_ORDER;
    decimal totalAmount = 100;
    decimal walletBalance = 200;

    var orderDto = new OrderDto
    {
        OrderId = orderId,
        CustomerId = userId,
        TotalAmount = totalAmount
    };

    var restaurant = new Restaurant
    {
        Uid = 123, // Mock a valid Uid for the restaurant
        NameRes = "Test Restaurant" // Set any other required properties
    };

    // Mock the order service to return a valid order
    _orderServiceMock.Setup(s => s.GetItemAsync(orderId, userId))
                     .ReturnsAsync(orderDto);

    // Mock the wallet service to return a sufficient wallet balance
    _walletServiceMock.Setup(w => w.GetBalanceOrderAsync(userId))
                      .ReturnsAsync(walletBalance);
    
    // Mock the wallet service to simulate a successful wallet balance update
    _walletServiceMock.Setup(w => w.UpdateWalletBalanceOrderAsync(It.IsAny<WalletBalanceOrderDto>()))
                      .ReturnsAsync(true);

    // Mock the update order status service to return true
    _orderServiceMock.Setup(s => s.UpdatePaidOrderAsync(It.IsAny<OrderDto>()))
                     .ReturnsAsync(true);

    // Mock the restaurant policy service to return a valid policy
    _restaurantPolicyServiceMock.Setup(s => s.GetActivePolicyAsync(restaurant.Uid))
                                .ReturnsAsync(new RestaurantPolicyDto
                                {
                                    Status = "ACTIVE",
                                    FirstFeePercent = 5, // Example fee percent
                                    ReturningFeePercent = 2
                                });

    // Mock the call to get restaurant details from a service
    

    // Act
    var result = await _controller.PaidOrder(orderId, userId, paymentGateway, typeOrder);

    // Assert
    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    var objectResult = (OkObjectResult)result;

    // Assert that the returned value is the expected success message
    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thanh toán đơn hàng thành công", objectResult.Value);
}

    }
}


