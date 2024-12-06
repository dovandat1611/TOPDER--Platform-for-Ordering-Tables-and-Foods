using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class HandleReportOrderTest
    {
        private Mock<IOrderService> _mockOrderService;
        private Mock<IOrderMenuService> _mockOrderMenuService;
        private Mock<IOrderTableService> _mockOrderTableService;
        private Mock<IWalletService> _mockWalletService;
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private Mock<IMenuRepository> _mockMenuRepository;
        private Mock<IRestaurantRepository> _mockRestaurantRepository;
        private Mock<IRestaurantService> _mockRestaurantService;
        private Mock<IUserService> _mockUserService;
        private Mock<IWalletTransactionService> _mockWalletTransactionService;
        private Mock<IPaymentGatewayService> _mockPaymentGatewayService;
        private Mock<ISendMailService> _mockSendMailService;
        private Mock<IDiscountMenuRepository> _mockDiscountMenuRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;
        private Mock<IRestaurantPolicyService> _mockRestaurantPolicyService;
        private Mock<IOrderRepository> _mockOrderRepository;

        private OrderController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mocking all services and repositories
            _mockOrderService = new Mock<IOrderService>();
            _mockOrderMenuService = new Mock<IOrderMenuService>();
            _mockOrderTableService = new Mock<IOrderTableService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _mockMenuRepository = new Mock<IMenuRepository>();
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            _mockRestaurantService = new Mock<IRestaurantService>();
            _mockUserService = new Mock<IUserService>();
            _mockWalletTransactionService = new Mock<IWalletTransactionService>();
            _mockPaymentGatewayService = new Mock<IPaymentGatewayService>();
            _mockSendMailService = new Mock<ISendMailService>();
            _mockDiscountMenuRepository = new Mock<IDiscountMenuRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();
            _mockRestaurantPolicyService = new Mock<IRestaurantPolicyService>();
            _mockOrderRepository = new Mock<IOrderRepository>();

            // Initializing the controller with mocked dependencies
            _controller = new OrderController(
                _mockOrderService.Object,
                _mockOrderMenuService.Object,
                _mockWalletService.Object,
                _mockMenuRepository.Object,
                _mockRestaurantRepository.Object,
                _mockDiscountRepository.Object,
                _mockUserService.Object,
                _mockWalletTransactionService.Object,
                _mockPaymentGatewayService.Object,
                _mockSendMailService.Object,
                _mockOrderTableService.Object,
                _mockDiscountMenuRepository.Object,
                _mockConfiguration.Object,
                _mockRestaurantService.Object,
                _mockNotificationService.Object,
                _mockSignalRHub.Object,
                _mockRestaurantPolicyService.Object,
                _mockOrderRepository.Object
            );
        }

        [TestMethod]
        public async Task HandleReportOrder_ReturnsBadRequest_WhenOrderNotFound()
        {
            // Arrange
            int orderID = -1;
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderID)); // Ensure null is of type OrderDto

            // Act
            var result = await _controller.HandleReportOrder(orderID);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đơn hàng đã bị thay đổi khi report! không thể xử lý.", badRequestResult.Value);
        }


        [TestMethod]
        public async Task HandleReportOrder_ReturnsBadRequest_WhenOrderStatusIsNotPaid()
        {
            // Arrange
            int orderID = 1;
            var order = new OrderDto { StatusOrder = Order_Status.CANCEL }; // Order is not PAID
            _mockOrderRepository.Setup(m => m.GetByIdAsync(orderID));

            // Act
            var result = await _controller.HandleReportOrder(orderID);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đơn hàng đã bị thay đổi khi report! không thể xử lý.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task HandleReportOrder_ReturnsOk_WhenOrderProcessedSuccessfully()
        {
            // Arrange
            int orderID = 1;
            var order = new OrderDto { StatusOrder = Order_Status.PAID, CustomerId = 1, RestaurantId = 1, TotalAmount = 100 };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderID));
            _mockOrderService.Setup(service => service.UpdateStatusAsync(orderID, Order_Status.CANCEL)).ReturnsAsync(order);
            _mockOrderService.Setup(service => service.GetInformationForCompleteAsync(orderID)).ReturnsAsync(new CompleteOrderDto
            {
                WalletId = 1,
                RestaurantID = 1,
                WalletBalance = 500,
                TotalAmount = 100
            });
            _mockWalletService.Setup(service => service.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>())).ReturnsAsync(true);
            _mockNotificationService.Setup(service => service.AddAsync(It.IsAny<NotificationDto>())).ReturnsAsync(new NotificationDto());
            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _mockSignalRHub.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);
            // Act
            var result = await _controller.HandleReportOrder(orderID);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật trạng thái cho đơn hàng với ID {orderID} thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task HandleReportOrder_ReturnsBadRequest_WhenOrderUpdateFails()
        {
            // Arrange
            int orderID = 1;
            var order = new OrderDto { StatusOrder = Order_Status.PAID };
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderID));
            _mockOrderService.Setup(service => service.UpdateStatusAsync(orderID, Order_Status.CANCEL)).ReturnsAsync((OrderDto)null); // Update fails

            // Act
            var result = await _controller.HandleReportOrder(orderID);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
        }

    }

}
