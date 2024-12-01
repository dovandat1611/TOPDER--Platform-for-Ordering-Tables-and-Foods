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
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetRestaurantPagingTest
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
        public async Task GetRestaurantPaging_ShouldReturnOk_WhenDataIsRetrievedSuccessfully()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = 1;
            string? status = "Completed";
            DateTime? month = new DateTime(2024, 11, 1);
            DateTime? date = null;

            var orders = new List<OrderRestaurantDto>
    {
        new OrderRestaurantDto { OrderId = 1, CustomerName = "Customer A", TotalAmount = 100 },
        new OrderRestaurantDto { OrderId = 2, CustomerName = "Customer B", TotalAmount = 200 }
    };
            var paginatedList = new PaginatedList<OrderRestaurantDto>(orders, orders.Count, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date))
                             .ReturnsAsync(paginatedList);

            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(It.IsAny<int>()))
                                 .ReturnsAsync(new List<OrderMenuDto>
                                 {
                             new OrderMenuDto { MenuId = 1, MenuName = "Menu A", Quantity = 2 }
                                 });

            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(It.IsAny<int>()))
                                  .ReturnsAsync(new List<OrderTableDto>
                                  {
                              new OrderTableDto { TableId = 1, TableName = "Table A" }
                                  });

            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId, status, month, date);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count());

            var firstOrder = response.Items.First();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Customer A", firstOrder.CustomerName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100, firstOrder.TotalAmount);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderMenus.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Menu A", firstOrder.OrderMenus.First().MenuName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderTables.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table A", firstOrder.OrderTables.First().TableName);
        }

        [TestMethod]
        public async Task GetRestaurantPaging_ShouldReturnOk_WhenDataAndMonthIsRetrievedSuccessfully()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = 1;
            string? status = "Completed";
            DateTime? month = new DateTime(2024, 11, 1);
            DateTime? date = new DateTime(2024, 11, 1);

            var orders = new List<OrderRestaurantDto>
    {
        new OrderRestaurantDto { OrderId = 1, CustomerName = "Customer A", TotalAmount = 100 },
        new OrderRestaurantDto { OrderId = 2, CustomerName = "Customer B", TotalAmount = 200 }
    };
            var paginatedList = new PaginatedList<OrderRestaurantDto>(orders, orders.Count, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date))
                             .ReturnsAsync(paginatedList);

            _orderMenuServiceMock.Setup(x => x.GetItemsByOrderAsync(It.IsAny<int>()))
                                 .ReturnsAsync(new List<OrderMenuDto>
                                 {
                             new OrderMenuDto { MenuId = 1, MenuName = "Menu A", Quantity = 2 }
                                 });

            _orderTableServiceMock.Setup(x => x.GetItemsByOrderAsync(It.IsAny<int>()))
                                  .ReturnsAsync(new List<OrderTableDto>
                                  {
                              new OrderTableDto { TableId = 1, TableName = "Table A" }
                                  });

            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId, status, month, date);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count());

            var firstOrder = response.Items.First();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Customer A", firstOrder.CustomerName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100, firstOrder.TotalAmount);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderMenus.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Menu A", firstOrder.OrderMenus.First().MenuName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, firstOrder.OrderTables.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Table A", firstOrder.OrderTables.First().TableName);
        }

        [TestMethod]
        public async Task GetRestaurantPaging_ShouldReturnOk_WithEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = 1;
            string? status = "Pending";
            DateTime? month = null;
            DateTime? date = null;

            var paginatedList = new PaginatedList<OrderRestaurantDto>(new List<OrderRestaurantDto>(), 0, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date))
                             .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId, status, month, date);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count());
        }

        [TestMethod]
        public async Task GetRestaurantPaging_ShouldReturnOk_WithEmptyList_WhenStatusIsNull()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int restaurantId = 456;
            string? status = "";
            DateTime? month = null;
            DateTime? date = null;

            var paginatedList = new PaginatedList<OrderRestaurantDto>(new List<OrderRestaurantDto>(), 0, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date))
                             .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId, status, month, date);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderRestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count());
        }

        [TestMethod]
        public async Task GetRestaurantPaging_ShouldThrowArgumentException_WhenPageSizeIsInvalid()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = -5; // Invalid page size
            int restaurantId = 456;
            string? status = null;
            DateTime? month = null;
            DateTime? date = null;

            _orderServiceMock.Setup(x => x.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId, status, month, date))
                             .ThrowsAsync(new ArgumentException("Page size must be greater than zero."));

            // Act & Assert
            await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _controller.GetRestaurantPaging(pageNumber, pageSize, restaurantId, status, month, date));
        }

    }
}
