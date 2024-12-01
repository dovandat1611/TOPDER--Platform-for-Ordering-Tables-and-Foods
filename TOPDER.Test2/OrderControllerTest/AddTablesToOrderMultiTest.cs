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
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class AddTablesToOrderMultiTest
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
        public async Task AddTablesToOrder_ReturnsBadRequest_WhenStatusOrdersIsNullOrEmpty()
        {
            // Arrange
            MultiStatusOrders invalidStatusOrders = null; // Simulate null request

            // Act
            var result = await _controller.AddTablesToOrder(invalidStatusOrders);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Yêu cầu không hợp lệ: Cần có thông tin đơn hàng và id đơn hàng.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTablesToOrder_ReturnsBadRequest_WhenOrderIDIsEmpty()
        {
            // Arrange
            var invalidStatusOrders = new MultiStatusOrders
            {
                OrderID = new List<int>(), // Empty OrderID list
                Status = "Pending"
            };

            // Act
            var result = await _controller.AddTablesToOrder(invalidStatusOrders);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Yêu cầu không hợp lệ: Cần có thông tin đơn hàng và id đơn hàng.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddTablesToOrder_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var validStatusOrders = new MultiStatusOrders
            {
                OrderID = new List<int> { 1, 2, 3 }, // Valid OrderID list
                Status = "Pending"
            };

            _mockOrderService
                .Setup(service => service.UpdatePendingOrdersAsync(validStatusOrders))
                .ReturnsAsync(true); // Simulate successful update

            // Act
            var result = await _controller.AddTablesToOrder(validStatusOrders);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật thành công.", okResult.Value);
        }

    }
}
