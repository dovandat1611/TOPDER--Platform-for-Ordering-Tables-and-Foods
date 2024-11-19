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
using TOPDER.Service.Dtos.Wallet;
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
        public async Task UpdateOrderStatus_ShouldReturnOk_WhenStatusUpdatedToConfirm()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.UpdateStatusAsync(1, Order_Status.CONFIRM))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetEmailForOrderAsync(1, User_Role.CUSTOMER))
                             .ReturnsAsync(new EmailForOrder
                             {
                                 Email = "customer@example.com",
                                 Name = "Customer Name",
                             });

            // Act
            var result = await _controller.UpdateOrderStatus(1, Order_Status.CONFIRM);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnOk_WhenStatusUpdatedToComplete()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.UpdateStatusAsync(1, Order_Status.COMPLETE))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetInformationForCompleteAsync(1))
                             .ReturnsAsync(new CompleteOrderDto
                             {
                                 WalletId = 101,
                                 RestaurantID = 1,
                                 WalletBalance = 500,
                                 TotalAmount = 100
                             });
            _walletServiceMock.Setup(x => x.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                              .ReturnsAsync(true);

            _orderServiceMock.Setup(x => x.GetEmailForOrderAsync(1, User_Role.CUSTOMER))
                            .ReturnsAsync(new EmailForOrder
                            {
                                Email = "customer@example.com",
                                Name = "Customer Name",
                            });

            // Act
            var result = await _controller.UpdateOrderStatus(1, Order_Status.COMPLETE);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }


    }
}
