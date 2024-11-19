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
                _configurationMock.Object
            );
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnBadRequest_WhenStatusIsNullOrEmpty()
        {
            // Arrange
            string status = null;
            int orderID = 123;

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnSuccess_WhenStatusIsCancelledAndUpdateIsSuccessful()
        {
            // Arrange
            string status = Payment_Status.CANCELLED;
            int orderID = 123;
            _orderServiceMock.Setup(x => x.UpdateStatusOrderPayment(orderID, status)).ReturnsAsync(true);

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnBadRequest_WhenStatusIsCancelledAndUpdateFails()
        {
            // Arrange
            string status = Payment_Status.CANCELLED;
            int orderID = 123;
            _orderServiceMock.Setup(x => x.UpdateStatusOrderPayment(orderID, status)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldSendEmail_WhenStatusIsSuccessfulAndUpdateIsSuccessful()
        {
            // Arrange
            string status = Payment_Status.SUCCESSFUL;
            int orderID = 123;
            var orderPaidEmail = new OrderPaidEmail { Email = "customer@example.com" };
            _orderServiceMock.Setup(x => x.UpdateStatusOrderPayment(orderID, status)).ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetOrderPaid(orderID)).ReturnsAsync(orderPaidEmail);

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnBadRequest_WhenStatusIsSuccessfulAndUpdateFails()
        {
            // Arrange
            string status = Payment_Status.SUCCESSFUL;
            int orderID = 123;

            // Mock the order update to fail
            _orderServiceMock.Setup(x => x.UpdateStatusOrderPayment(orderID, status)).ReturnsAsync(false);

            // Mock GetOrderPaid to return a valid OrderPaidEmail
            var orderPaidEmail = new OrderPaidEmail { Email = "customer@example.com" };
            _orderServiceMock.Setup(x => x.GetOrderPaid(orderID)).ReturnsAsync(orderPaidEmail);

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);

        }

        [TestMethod]
        public async Task GetItemAsync_ShouldReturnBadRequest_WhenStatusIsInvalid()
        {
            // Arrange
            string status = "InvalidStatus";
            int orderID = 123;

            // Act
            var result = await _controller.GetItemAsync(orderID, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
        }
    }
}
