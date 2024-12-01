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
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class GetCustomerPagingTest
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
        public async Task GetCustomerPaging_ShouldReturnOk_WhenDataIsRetrievedSuccessfully()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int customerId = 123;
            string? status = "Completed";

            var orders = new List<OrderCustomerDto>
    {
        new OrderCustomerDto { OrderId = 1, RestaurantName = "Restaurant A", TotalAmount = 100 },
        new OrderCustomerDto { OrderId = 2, RestaurantName = "Restaurant B", TotalAmount = 200 }
    };
            var paginatedList = new PaginatedList<OrderCustomerDto>(orders, orders.Count, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetCustomerPagingAsync(pageNumber, pageSize, customerId, status))
                             .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetCustomerPaging(pageNumber, pageSize, customerId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Items.Count());
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant A", response.Items.First().RestaurantName);
        }
        [TestMethod]
        public async Task GetCustomerPaging_ShouldReturnOk_WithEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            int customerId = -1;
            string? status = "Pending";

            var paginatedList = new PaginatedList<OrderCustomerDto>(new List<OrderCustomerDto>(), 0, pageNumber, pageSize);

            _orderServiceMock.Setup(x => x.GetCustomerPagingAsync(pageNumber, pageSize, customerId, status))
                             .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetCustomerPaging(pageNumber, pageSize, customerId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<OrderCustomerDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count());
        }
        [TestMethod]
        public async Task GetCustomerPaging_ShouldThrowArgumentException_WhenInvalidPageSizeIsProvided()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 1; // Invalid page size
            int customerId = 123;
            string? status = null;

            _orderServiceMock.Setup(x => x.GetCustomerPagingAsync(pageNumber, pageSize, customerId, status))
                             .ThrowsAsync(new ArgumentException("Page size must be greater than zero."));

            // Act & Assert
            await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _controller.GetCustomerPaging(pageNumber, pageSize, customerId, status));
        }

    }
}
