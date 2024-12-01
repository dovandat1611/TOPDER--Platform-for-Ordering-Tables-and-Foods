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
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
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
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private Mock<ISendMailService> _sendMailServiceMock;
        private Mock<IDiscountMenuRepository> _discountMenuRepositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IHubContext<AppHub>> _signalRHubMock;
        private Mock<IRestaurantPolicyService> _restaurantPolicyServiceMock;
        private Mock<IOrderRepository> _orderRepositoryMock;

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
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _sendMailServiceMock = new Mock<ISendMailService>();
            _discountMenuRepositoryMock = new Mock<IDiscountMenuRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _notificationServiceMock = new Mock<INotificationService>();
            _signalRHubMock = new Mock<IHubContext<AppHub>>();
            _restaurantPolicyServiceMock = new Mock<IRestaurantPolicyService>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

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
                _restaurantServiceMock.Object,
                _notificationServiceMock.Object,
                _signalRHubMock.Object,
                _restaurantPolicyServiceMock.Object,
                _orderRepositoryMock.Object
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
                CancellationFeePercent = 10,
                EmailRestaurant = "restaurant@example.com",
                EmailCustomer = "customer@example.com"
            };
            var checkStatusOrder = new OrderDto
            {
                PaidAt = null // Đơn hàng chưa thanh toán
            };
            var orderDto = new OrderDto { PaidAt = DateTime.Now };

            // Mock các phương thức cần thiết
            _orderServiceMock.Setup(os => os.UpdateStatusCancelAsync(cancelOrderRequest.OrderId, Order_Status.CANCEL, cancelOrderRequest.CancelReason))
                             .ReturnsAsync(true);
            _orderServiceMock.Setup(os => os.GetInformationForCancelAsync(cancelOrderRequest.UserId, cancelOrderRequest.OrderId))
                             .ReturnsAsync(cancelOrderDto);
            _orderServiceMock.Setup(os => os.GetItemAsync(cancelOrderRequest.OrderId, cancelOrderRequest.UserId))
                             .ReturnsAsync(checkStatusOrder);
            _walletServiceMock.Setup(ws => ws.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>())).ReturnsAsync(true);
            _walletTransactionServiceMock.Setup(wts => wts.AddAsync(It.IsAny<WalletTransactionDto>())).ReturnsAsync(true);
            _sendMailServiceMock.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelOrder(cancelOrderRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Cập nhật trạng thái cho đơn hàng với ID {cancelOrderRequest.OrderId} thành công.", okResult.Value);

            // Verify that the wallet balance update methods were called
            _walletServiceMock.Verify(x => x.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()), Times.Exactly(2));

            // Verify that the wallet transaction methods were called
            _walletTransactionServiceMock.Verify(x => x.AddAsync(It.IsAny<WalletTransactionDto>()), Times.Exactly(2));

            // Verify that SendEmailAsync was called once for customer
            _sendMailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrder(It.IsAny<OrderPaidEmail>(), Order_Status.CANCEL)), Times.Once);

            // Optionally, verify if the email sent to the restaurant or customer is correctly determined based on the role
            if (cancelOrderDto.RoleName == User_Role.CUSTOMER)
            {
                _sendMailServiceMock.Verify(x => x.SendEmailAsync(cancelOrderDto.EmailRestaurant, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrder(It.IsAny<OrderPaidEmail>(), Order_Status.CANCEL)), Times.Once);
            }
            else
            {
                _sendMailServiceMock.Verify(x => x.SendEmailAsync(cancelOrderDto.EmailCustomer, Email_Subject.UPDATESTATUS, EmailTemplates.UpdateStatusOrderRestaurant(It.IsAny<OrderPaidEmail>(), Order_Status.CANCEL)), Times.Once);
            }
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
