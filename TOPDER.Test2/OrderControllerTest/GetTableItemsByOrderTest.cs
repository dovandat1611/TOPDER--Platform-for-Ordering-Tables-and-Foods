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
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetTableItemsByOrderTest
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
        public async Task GetTableItemsByOrder_ShouldReturnNotFound_WhenNoTablesFound()
        {
            // Arrange
            int orderId = -1;
            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync((List<OrderTableDto>)null); // Không có bàn

            // Act
            var result = await _controller.GetTableItemsByOrder(orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy bàn cho đơn hàng này.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetTableItemsByOrder_ShouldReturnOk_WhenTablesFound()
        {
            // Arrange
            int orderId = 1;
            var tables = new List<OrderTableDto>
        {
            new OrderTableDto { TableId = 1, OrderId = orderId, TableName = "Table 1" },
            new OrderTableDto { TableId = 2, OrderId = orderId, TableName = "Table 2" }
        };

            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync(tables); // Mock trả về danh sách bàn

            // Act
            var result = await _controller.GetTableItemsByOrder(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as List<OrderTableDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue);
                    Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(2, returnValue.Count); // Kiểm tra số lượng bàn
        }

        [TestMethod]
        public async Task GetTableItemsByOrder_ShouldReturnNotFound_WhenTablesListIsEmpty()
        {
            // Arrange
            int orderId = 1;
            var emptyTableList = new List<OrderTableDto>(); // Danh sách bàn rỗng

            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(orderId))
                                  .ReturnsAsync(emptyTableList); // Mock trả về danh sách bàn rỗng

            // Act
            var result = await _controller.GetTableItemsByOrder(orderId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy bàn cho đơn hàng này.", notFoundResult.Value);
        }
    }
}
