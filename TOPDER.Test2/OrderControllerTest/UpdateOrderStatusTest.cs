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
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class UpdateOrderStatusTest
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
        public async Task UpdateOrderStatus_ShouldReturnBadRequest_WhenStatusIsNullOrEmpty()
        {
            // Arrange
            int orderId = 1;
            string status = null!;

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Trạng thái không thể để trống.", badRequestResult.Value);
        }
        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnBadRequest_WhenStatusIsInvalid()
        {
            // Arrange
            int orderId = 1;
            string status = "INVALID_STATUS";

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Trạng thái không hợp lệ (Confirm | Complete).", badRequestResult.Value);
        }
        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 999;
            string status = Order_Status.CONFIRM;

            _orderServiceMock.Setup(x => x.UpdateStatusAsync(orderId, status))
                             .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, status);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Đơn hàng với ID {orderId} không tồn tại hoặc trạng thái không thay đổi.", notFoundResult.Value);
        }
        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnBadRequest_WhenStatusIsEmpty()
        {
            // Act
            var result = await _controller.UpdateOrderStatus(1, "");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Trạng thái không thể để trống.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnBadRequest1_WhenStatusIsInvalid()
        {
            // Act
            var result = await _controller.UpdateOrderStatus(1, "INVALID");

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Trạng thái không hợp lệ (Confirm | Complete).", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnNotFound1_WhenOrderDoesNotExist()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.UpdateStatusAsync(1, Order_Status.CONFIRM))
                             .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateOrderStatus(1, Order_Status.CONFIRM);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đơn hàng với ID 1 không tồn tại hoặc trạng thái không thay đổi.", notFoundResult.Value);
        }
        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnOk_WhenStatusIsConfirm()
        {
            // Arrange
            int orderID = 1;
            string status = Order_Status.CONFIRM;

            _orderServiceMock.Setup(os => os.UpdateStatusAsync(orderID, status)).ReturnsAsync(true);
            _orderServiceMock.Setup(os => os.GetEmailForOrderAsync(orderID, User_Role.CUSTOMER))
                             .ReturnsAsync(new EmailForOrder { Email = "customer@example.com" });
            _orderServiceMock.Setup(os => os.GetOrderPaid(orderID)).ReturnsAsync(new OrderPaidEmail { OrderId = orderID.ToString() });
            _sendMailServiceMock.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrderStatus(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _orderServiceMock.Verify(os => os.UpdateStatusAsync(orderID, status), Times.Once);
            _sendMailServiceMock.Verify(sms => sms.SendEmailAsync(It.IsAny<string>(), Email_Subject.UPDATESTATUS, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnOk_WhenStatusIsComplete_AndWalletUpdateIsSuccessful()
        {
            // Arrange
            int orderID = 1;
            string status = Order_Status.COMPLETE;

            // Mock dữ liệu trả về từ các service
            _orderServiceMock.Setup(os => os.UpdateStatusAsync(orderID, status)).ReturnsAsync(true);
            _orderServiceMock.Setup(os => os.GetEmailForOrderAsync(orderID, User_Role.CUSTOMER))
                             .ReturnsAsync(new EmailForOrder { Email = "customer@example.com" });
            _orderServiceMock.Setup(os => os.GetOrderPaid(orderID))
                             .ReturnsAsync(new OrderPaidEmail
                             {
                                 OrderId = orderID.ToString(),
                                 TotalAmount = 100
                             });
            _orderServiceMock.Setup(os => os.GetInformationForCompleteAsync(orderID))
                             .ReturnsAsync(new CompleteOrderDto
                             {
                                 WalletId = 1,
                                 RestaurantID = 1,
                                 WalletBalance = 1000,
                                 TotalAmount = 100
                             });
            _walletServiceMock.Setup(ws => ws.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>())).ReturnsAsync(true);
            _walletTransactionServiceMock.Setup(wts => wts.AddAsync(It.IsAny<WalletTransactionDto>())).ReturnsAsync(true);
            _sendMailServiceMock.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrderStatus(orderID, status);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // Kiểm tra các mock được gọi đúng số lần
            _orderServiceMock.Verify(os => os.UpdateStatusAsync(orderID, status), Times.Once);
            _orderServiceMock.Verify(os => os.GetInformationForCompleteAsync(orderID), Times.Once);
            _walletServiceMock.Verify(ws => ws.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()), Times.Once);
            _walletTransactionServiceMock.Verify(wts => wts.AddAsync(It.IsAny<WalletTransactionDto>()), Times.Once);
            _sendMailServiceMock.Verify(sms => sms.SendEmailAsync(It.IsAny<string>(), Email_Subject.UPDATESTATUS, It.IsAny<string>()), Times.Once);
        }
    }
}
