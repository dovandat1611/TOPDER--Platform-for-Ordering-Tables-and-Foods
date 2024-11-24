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
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class CheckPaymentTest
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
        public async Task CheckPayment_StatusCancelled_ReturnsOk()
        {
            // Arrange
            int orderID = 1;
            string status = Payment_Status.CANCELLED;

            _orderServiceMock
                .Setup(service => service.UpdateStatusOrderPayment(orderID, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckPayment(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);
        }
        [TestMethod]
        public async Task CheckPayment_StatusSuccessful_ReturnsOkAndSendsEmail()
        {
            // Arrange
            int orderID = 1;
            string status = Payment_Status.SUCCESSFUL;

            var orderPaidEmail = new OrderPaidEmail
            {
                Email = "customer@example.com",
            };

            _orderServiceMock
                .Setup(service => service.UpdateStatusOrderPayment(orderID, status))
                .ReturnsAsync(true);

            _orderServiceMock
                .Setup(service => service.GetOrderPaid(orderID))
                .ReturnsAsync(orderPaidEmail);

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(orderPaidEmail.Email, Email_Subject.ORDERCONFIRM, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CheckPayment(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);

            // Verify email sending
            _sendMailServiceMock.Verify(service =>
                service.SendEmailAsync(orderPaidEmail.Email, Email_Subject.ORDERCONFIRM, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task CheckPayment_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            int orderID = 1;
            string status = "INVALID_STATUS";

            // Act
            var result = await _controller.CheckPayment(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }
        [TestMethod]
        public async Task CheckPayment_NullStatus_ReturnsBadRequest()
        {
            // Arrange
            int orderID = 1;
            string? status = null;

            // Act
            var result = await _controller.CheckPayment(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(badRequestResult?.Value);
        }

        [TestMethod]
        public async Task CheckPayment_OrderIdDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            int nonExistentOrderID = -1; // Giả định đây là ID không tồn tại
            string status = Payment_Status.CANCELLED;

            _orderServiceMock
                .Setup(service => service.UpdateStatusOrderPayment(nonExistentOrderID, status))
                .ReturnsAsync(false); // Trả về false khi orderID không tồn tại

            // Act
            var result = await _controller.CheckPayment(nonExistentOrderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult?.Value);
        }
    }
}
