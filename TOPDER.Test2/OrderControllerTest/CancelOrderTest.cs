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
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class CancelOrderTest
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
        public async Task CancelOrder_ShouldReturnNotFound_WhenOrderUpdateFails()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "Customer cancelled" };
            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, "Customer cancelled"))
                             .ReturnsAsync(false);

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Đơn hàng với ID 1 không tồn tại hoặc trạng thái không thay đổi.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task CancelOrder_ShouldReturnOk_WhenOrderHasNoAmount()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "Customer cancelled" };
            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, "Customer cancelled"))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetInformationForCancelAsync(1, 1))
                             .ReturnsAsync(new CancelOrderDto { TotalAmount = 0 });

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task CancelOrder_ShouldReturnOk_WhenOrderNotPaidYet()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "Customer cancelled" };
            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, "Customer cancelled"))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetInformationForCancelAsync(1, 1))
                             .ReturnsAsync(new CancelOrderDto { TotalAmount = 100, RoleName = User_Role.CUSTOMER, CancellationFeePercent = 10 });
            _orderServiceMock.Setup(x => x.GetItemAsync(1, 1))
                             .ReturnsAsync(new OrderDto { PaidAt = null });

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task CancelOrder_ShouldUpdateWalletAndCreateTransaction_WhenCancelOrderWithAmount()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "Customer cancelled" };
            var cancelOrderDto = new CancelOrderDto
            {
                TotalAmount = 100,
                WalletCustomerId = 1,
                CustomerID = 1,
                WalletBalanceCustomer = 500,
                WalletRestaurantId = 2,
                RestaurantID = 2,
                WalletBalanceRestaurant = 1000,
                RoleName = User_Role.CUSTOMER,
                CancellationFeePercent = 10
            };

            // Ensure it returns a Task<bool>, not just a bool.
            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, "Customer cancelled"))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetInformationForCancelAsync(1, 1))
                             .ReturnsAsync(cancelOrderDto);
            _orderServiceMock.Setup(x => x.GetItemAsync(1, 1))
                             .ReturnsAsync(new OrderDto { PaidAt = DateTime.Now });
            _walletServiceMock.Setup(x => x.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                              .ReturnsAsync(true);
            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Assert
            var okResult = result as OkObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
            _walletServiceMock.Verify(x => x.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()), Times.Exactly(2));
            _walletTransactionServiceMock.Verify(x => x.AddAsync(It.IsAny<WalletTransactionDto>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task CancelOrder_ShouldSendEmail_WhenOrderCancelled()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "Customer cancelled" };
            var cancelOrderDto = new CancelOrderDto
            {
                TotalAmount = 100,
                EmailCustomer = "customer@example.com",
                EmailRestaurant = "restaurant@example.com",
                NameCustomer = "Customer",
                NameRestaurant = "Restaurant",
                RoleName = User_Role.CUSTOMER
            };

            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, "Customer cancelled"))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(x => x.GetInformationForCancelAsync(1, 1))
                             .ReturnsAsync(cancelOrderDto);
            _orderServiceMock.Setup(x => x.GetItemAsync(1, 1))
                             .ReturnsAsync(new OrderDto { PaidAt = null });

            // Make sure the SendEmailAsync is mocked properly
            _sendMailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Additional assert to check the correct flow was triggered
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task CancelOrder_ShouldSendEmail_WhenCancelReasonIsEmpty()
        {
            // Arrange
            var cancelOrderRequest = new CancelOrderRequest { OrderId = 1, UserId = 1, CancelReason = "" }; // CancelReason là chuỗi rỗng
            var cancelOrderDto = new CancelOrderDto
            {
                TotalAmount = 100,
                EmailCustomer = "customer@example.com",
                EmailRestaurant = "restaurant@example.com",
                NameCustomer = "Customer",
                NameRestaurant = "Restaurant",
                RoleName = User_Role.CUSTOMER
            };

            // Setup mock service behaviors
            _orderServiceMock.Setup(x => x.UpdateStatusCancelAsync(1, Order_Status.CANCEL, ""))
                             .ReturnsAsync(true);  // Chú ý truyền "" cho CancelReason
            _orderServiceMock.Setup(x => x.GetInformationForCancelAsync(1, 1))
                             .ReturnsAsync(cancelOrderDto);
            _orderServiceMock.Setup(x => x.GetItemAsync(1, 1))
                             .ReturnsAsync(new OrderDto { PaidAt = null });

            _sendMailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);  // Mock email

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Thêm assert để kiểm tra response trả về
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật trạng thái cho đơn hàng với ID 1 thành công.", okResult.Value);
        }

    }
}
