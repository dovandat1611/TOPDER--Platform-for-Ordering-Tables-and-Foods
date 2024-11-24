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
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class CalculateTotalAmountFreDiscountAsyncTest
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
        public async Task CalculateTotalAmountFreDiscountAsync_ShouldReturnNotFound_WhenRestaurantNotFound()
        {
            // Arrange
            var caculatorOrder = new CaculatorOrderDto
            {
                CustomerId = 1,
                RestaurantId = -1, // Non-existent restaurant ID
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } }
            };

            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant)null);

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

            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(restaurant);
            // Act
            var result = await _controller.CalculateTotalAmountFreDiscountAsync(caculatorOrder);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // StatusCode should be 200
        }
    }
}
