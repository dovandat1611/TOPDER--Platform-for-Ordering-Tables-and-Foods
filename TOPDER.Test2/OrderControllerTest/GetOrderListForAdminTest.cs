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
    public class GetOrderListForAdminTest
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
        public async Task GetOrderListForAdmin_ReturnsCorrectOrderList()
        {
            // Arrange
            var mockOrders = new List<OrderDto>
    {
        new OrderDto
        {
            OrderId = 1,
            CustomerId = 101,
            NameReceiver = "John Doe",
            PhoneReceiver = "123456789",
            TimeReservation = new TimeSpan(18, 30, 0),
            DateReservation = DateTime.Today,
            NumberPerson = 4,
            NumberChild = 2,
            StatusOrder = "Confirmed",
            OrderTables = new List<OrderTableDto>(),
            OrderMenus = new List<OrderMenuDto>()
        },
        new OrderDto
        {
            OrderId = 2,
            CustomerId = 102,
            NameReceiver = "Jane Smith",
            PhoneReceiver = "987654321",
            TimeReservation = new TimeSpan(19, 0, 0),
            DateReservation = DateTime.Today.AddDays(1),
            NumberPerson = 6,
            NumberChild = 0,
            StatusOrder = "Pending",
            OrderTables = new List<OrderTableDto>(),
            OrderMenus = new List<OrderMenuDto>()
        }
    };

            var mockOrderMenus = new List<OrderMenuDto>
    {
        new OrderMenuDto { OrderMenuId = 1, OrderId = 1, MenuId = 201, MenuName = "Pizza", MenuImage = "pizza.jpg", Quantity = 2, Price = 15.00m },
        new OrderMenuDto { OrderMenuId = 2, OrderId = 2, MenuId = 202, MenuName = "Burger", MenuImage = "burger.jpg", Quantity = 1, Price = 10.00m }
    };

            var mockOrderTables = new List<OrderTableDto>
    {
        new OrderTableDto { OrderTableId = 1, OrderId = 1, TableId = 301, TableName = "Table 1", MaxCapacity = 4 },
        new OrderTableDto { OrderTableId = 2, OrderId = 2, TableId = 302, TableName = "Table 2", MaxCapacity = 6 }
    };

            _orderServiceMock
                .Setup(service => service.GetAdminPagingAsync())
                .ReturnsAsync(mockOrders);

            _orderMenuServiceMock
                .Setup(service => service.GetItemsByOrderAsync(It.IsAny<int>()))
                .ReturnsAsync((int orderId) => mockOrderMenus.Where(x => x.OrderId == orderId).ToList());

            _orderTableServiceMock
                .Setup(service => service.GetItemsByOrderAsync(It.IsAny<int>()))
                .ReturnsAsync((int orderId) => mockOrderTables.Where(x => x.OrderId == orderId).ToList());

            // Act
            var result = await _controller.GetOrderListForAdmin();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult?.Value);

            var orders = okResult.Value as List<OrderDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(orders);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, orders.Count);

            // Verify the first order details
            var firstOrder = orders.First();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(101, firstOrder.CustomerId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderMenus.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Pizza", firstOrder.OrderMenus.First().MenuName);
                   Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderTables.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table 1", firstOrder.OrderTables.First().TableName);

            // Verify the second order details
            var secondOrder = orders.Last();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, secondOrder.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(102, secondOrder.CustomerId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, secondOrder.OrderMenus.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Burger", secondOrder.OrderMenus.First().MenuName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, secondOrder.OrderTables.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table 2", secondOrder.OrderTables.First().TableName);
        }


    }
}
