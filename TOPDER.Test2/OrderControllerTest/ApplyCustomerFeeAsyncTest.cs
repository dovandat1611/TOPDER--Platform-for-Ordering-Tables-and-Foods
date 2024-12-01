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
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.RestaurantPolicy;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class ApplyCustomerFeeAsyncTest
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
        public async Task ApplyCustomerFeeAsync_ReturnsZero_WhenRestaurantPolicyIsNull()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = 1;

            // Simulate no active policy for the restaurant
            _mockRestaurantPolicyService.Setup(s => s.GetActivePolicyAsync(It.IsAny<int>())).ReturnsAsync((RestaurantPolicyDto)null);

            // Act
            var result = await _controller.ApplyCustomerFeeAsync(customerId, restaurantId);

            // Assert
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task ApplyCustomerFeeAsync_ReturnsFirstFeePercent_WhenIsFirstOrderAndFirstFeePercentIsGreaterThanZero()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = 1;
            var restaurantPolicy = new RestaurantPolicyDto
            {
                FirstFeePercent = 10,
                ReturningFeePercent = 5
            };

            // Simulate that the restaurant has a valid active policy
            _mockRestaurantPolicyService.Setup(s => s.GetActivePolicyAsync(It.IsAny<int>())).ReturnsAsync(restaurantPolicy);

            // Simulate that it's the customer's first order
            _mockOrderService.Setup(s => s.CheckIsFirstOrderAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _controller.ApplyCustomerFeeAsync(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10, result); // Should return FirstFeePercent
        }

        [TestMethod]
        public async Task ApplyCustomerFeeAsync_ReturnsReturningFeePercent_WhenNotFirstOrderAndReturningFeePercentIsGreaterThanZero()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = 1;
            var restaurantPolicy = new RestaurantPolicyDto
            {
                FirstFeePercent = 10,
                ReturningFeePercent = 5
            };

            // Simulate that the restaurant has a valid active policy
            _mockRestaurantPolicyService.Setup(s => s.GetActivePolicyAsync(It.IsAny<int>())).ReturnsAsync(restaurantPolicy);

            // Simulate that it's not the customer's first order
            _mockOrderService.Setup(s => s.CheckIsFirstOrderAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _controller.ApplyCustomerFeeAsync(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, result); // Should return ReturningFeePercent
        }

        [TestMethod]
        public async Task ApplyCustomerFeeAsync_ReturnsZero_WhenFeePercentIsZero()
        {
            // Arrange
            int customerId = 1;
            int restaurantId = 1;
            var restaurantPolicy = new RestaurantPolicyDto
            {
                FirstFeePercent = 0,
                ReturningFeePercent = 0
            };

            // Simulate that the restaurant has a valid active policy
            _mockRestaurantPolicyService.Setup(s => s.GetActivePolicyAsync(It.IsAny<int>())).ReturnsAsync(restaurantPolicy);

            // Act
            var result = await _controller.ApplyCustomerFeeAsync(customerId, restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result); // Both fees are 0, should return 0
        }

        
    }
}
