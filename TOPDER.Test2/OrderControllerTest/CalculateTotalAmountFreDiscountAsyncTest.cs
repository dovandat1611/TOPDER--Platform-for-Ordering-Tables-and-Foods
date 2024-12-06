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
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class CalculateTotalAmountFreDiscountAsyncTest
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
        public async Task CalculateTotalAmountFreDiscountAsync_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("CustomerId", "Required");

            var caculatorOrder = new CaculatorOrderDto
            {
                CustomerId = 0, // Invalid ID to trigger ModelState error
                RestaurantId = 1,
                OrderMenus = new List<OrderMenuModelDto>()
            };

            // Act
            var result = await _controller.CalculateTotalAmountFreDiscountAsync(caculatorOrder);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // StatusCode should be 400
        }

        [TestMethod]
        public async Task CalculateTotalAmountFreDiscountAsync_ShouldReturnBadRequest_WhenOrderMenusIsEmpty()
        {
            // Arrange
            _controller.ModelState.AddModelError("OrderMenus", "Required");

            var caculatorOrder = new CaculatorOrderDto
            {
                CustomerId = 1, // Invalid ID to trigger ModelState error
                RestaurantId = 1,
                OrderMenus = new List<OrderMenuModelDto>()
            };

            // Act
            var result = await _controller.CalculateTotalAmountFreDiscountAsync(caculatorOrder);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // StatusCode should be 400
        }

        [TestMethod]
        public async Task CalculateTotalAmountFreDiscountAsync_ShouldReturnNotFound_WhenRestaurantNotFound()
        {
            // Arrange
            var caculatorOrder = new CaculatorOrderDto
            {
                CustomerId = 1,
                RestaurantId = -1, // Non-existent restaurant ID
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } }
            };

            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant)null);

            // Act
            var result = await _controller.CalculateTotalAmountFreDiscountAsync(caculatorOrder);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode); // StatusCode should be 404
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Nhà hàng không tồn tại.", notFoundResult.Value); // Message should match
        }

        [TestMethod]
        public async Task CalculateTotalAmountFreDiscountAsync_ShouldReturnOkWithTotalAmount_WhenRestaurantFound()
        {
            // Arrange
            var caculatorOrder = new CaculatorOrderDto
            {
                CustomerId = 1,
                RestaurantId = 1,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } }
            };

            var restaurant = new Restaurant
            {
                Uid = 1,
                Price = 100m,
                Discount = 10
            };

            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(restaurant);
            // Act
            var result = await _controller.CalculateTotalAmountFreDiscountAsync(caculatorOrder);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // StatusCode should be 200
        }
    }
}
