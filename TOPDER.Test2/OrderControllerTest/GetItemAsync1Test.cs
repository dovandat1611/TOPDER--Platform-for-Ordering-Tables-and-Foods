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
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetItemAsync1Test
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
        public async Task GetItemAsync_ShouldReturnOk_WhenOrderDetailsAreRetrievedSuccessfully()
        {
            // Arrange
            int Uid = 1;
            int orderId = 123;

            var orderDto = new OrderDto { OrderId = orderId };
            var orderTables = new List<OrderTableDto> { new OrderTableDto { TableId = 1 } };
            var orderMenus = new List<OrderMenuDto> { new OrderMenuDto { MenuId = 1 } };

            _orderServiceMock.Setup(x => x.GetItemAsync(orderId, Uid)).ReturnsAsync(orderDto);
            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId)).ReturnsAsync(orderTables);
            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId)).ReturnsAsync(orderMenus);

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnedOrderDto = okResult.Value as OrderDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedOrderDto);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderId, returnedOrderDto.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderTables, returnedOrderDto.OrderTables);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(orderMenus, returnedOrderDto.OrderMenus);
        }
        [TestMethod]
        public async Task GetItemAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int Uid = 1;
            int orderId = 123;

            _orderServiceMock.Setup(x => x.GetItemAsync(orderId, Uid)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Đơn hàng với ID {orderId} không tồn tại.", notFoundResult.Value);
        }
        [TestMethod]
        public async Task GetItemAsync_ShouldReturnForbid_WhenUserIsUnauthorized()
        {
            // Arrange
            int Uid = 1;
            int orderId = 123;

            _orderServiceMock.Setup(x => x.GetItemAsync(orderId, Uid)).ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetItemAsync(Uid, orderId);

            // Assert
            var forbidResult = result as ForbidResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(forbidResult);
        }

    }
}
