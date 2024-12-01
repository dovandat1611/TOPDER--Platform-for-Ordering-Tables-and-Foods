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
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Hubs;
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
            int orderId = -1;
            string status = Order_Status.CONFIRM;

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, status);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual($"Đơn hàng với ID {orderId} không tồn tại hoặc trạng thái không thay đổi.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateOrderStatus_ShouldReturnOk_WhenStatusIsConfirm()
        {
            // Arrange
            int orderID = 1;
            string status = Order_Status.CONFIRM;
            var order = new OrderDto
            {
                OrderId = 1,
                CustomerId = 123,
                RestaurantId = 456,
                DiscountId = null,
                NameReceiver = "Nguyễn Văn A",
                PhoneReceiver = "0123456789",
                TimeReservation = new TimeSpan(18, 30, 0), // 6:30 PM
                DateReservation = DateTime.Now.AddDays(1), // Ngày mai
                NumberPerson = 4,
                NumberChild = 2,
                ContentReservation = "Sinh nhật",
                TypeOrder = "Dine-in",
                PaidType = "Credit Card",
                DepositAmount = 500000, // Tiền cọc
                FoodAmount = 2000000, // Tổng tiền đồ ăn
                FoodAddAmount = 300000, // Tổng tiền thêm
                TotalAmount = 2800000, // Tổng cộng
                ContentPayment = "Thanh toán qua thẻ",
                StatusPayment = "Paid",
                StatusOrder = "CONFIRM",
                CreatedAt = DateTime.Now,
                ConfirmedAt = DateTime.Now.AddHours(1),
                PaidAt = DateTime.Now.AddHours(2),
                CompletedAt = null,
                CancelledAt = null,
                CancelReason = null,
                OrderTables = new List<OrderTableDto>
                {
                    new OrderTableDto
                    {
                        OrderTableId = 1,
                        OrderId = 1,
                        TableId = 101,
                        RoomId = 1,
                        RoomName = "VIP Room",
                        TableName = "Table 1",
                        MaxCapacity = 6
                    }
                },
                OrderMenus = new List<OrderMenuDto>
                {
                    new OrderMenuDto
                    {
                        OrderMenuId = 1,
                        OrderId = 1,
                        MenuId = 201,
                        MenuName = "Bò bít tết",
                        MenuImage = "steak.jpg",
                        Quantity = 2,
                        Price = 250000,
                        OrderMenuType = "Main Course"
                    }
                },
                OrderMenusAdd = new List<OrderMenuDto>
                {
                    new OrderMenuDto
                    {
                        OrderMenuId = 2,
                        OrderId = 1,
                        MenuId = 202,
                        MenuName = "Nước ngọt",
                        MenuImage = "softdrink.jpg",
                        Quantity = 3,
                        Price = 20000,
                        OrderMenuType = "Drink"
                    }
                }
            };

            _orderServiceMock.Setup(os => os.UpdateStatusAsync(orderID, status)).ReturnsAsync(order);
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
            var order = new OrderDto
            {
                OrderId = 1,
                CustomerId = 123,
                RestaurantId = 456,
                DiscountId = null,
                NameReceiver = "Nguyễn Văn A",
                PhoneReceiver = "0123456789",
                TimeReservation = new TimeSpan(18, 30, 0), // 6:30 PM
                DateReservation = DateTime.Now.AddDays(1), // Ngày mai
                NumberPerson = 4,
                NumberChild = 2,
                ContentReservation = "Sinh nhật",
                TypeOrder = "Dine-in",
                PaidType = "Credit Card",
                DepositAmount = 500000, // Tiền cọc
                FoodAmount = 2000000, // Tổng tiền đồ ăn
                FoodAddAmount = 300000, // Tổng tiền thêm
                TotalAmount = 2800000, // Tổng cộng
                ContentPayment = "Thanh toán qua thẻ",
                StatusPayment = "Paid",
                StatusOrder = "COMPLETE",
                CreatedAt = DateTime.Now,
                ConfirmedAt = DateTime.Now.AddHours(1),
                PaidAt = DateTime.Now.AddHours(2),
                CompletedAt = null,
                CancelledAt = null,
                CancelReason = null,
                OrderTables = new List<OrderTableDto>
                {
                    new OrderTableDto
                    {
                        OrderTableId = 1,
                        OrderId = 1,
                        TableId = 101,
                        RoomId = 1,
                        RoomName = "VIP Room",
                        TableName = "Table 1",
                        MaxCapacity = 6
                    }
                },
                OrderMenus = new List<OrderMenuDto>
                {
                    new OrderMenuDto
                    {
                        OrderMenuId = 1,
                        OrderId = 1,
                        MenuId = 201,
                        MenuName = "Bò bít tết",
                        MenuImage = "steak.jpg",
                        Quantity = 2,
                        Price = 250000,
                        OrderMenuType = "Main Course"
                    }
                },
                OrderMenusAdd = new List<OrderMenuDto>
                {
                    new OrderMenuDto
                    {
                        OrderMenuId = 2,
                        OrderId = 1,
                        MenuId = 202,
                        MenuName = "Nước ngọt",
                        MenuImage = "softdrink.jpg",
                        Quantity = 3,
                        Price = 20000,
                        OrderMenuType = "Drink"
                    }
                }
            };

            // Mock dữ liệu trả về từ các service
            _orderServiceMock.Setup(os => os.UpdateStatusAsync(orderID, status)).ReturnsAsync(order);
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
