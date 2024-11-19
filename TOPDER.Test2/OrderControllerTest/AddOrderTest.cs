using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class AddOrderTest
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
            // Initialize mock dependencies
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

            // Inject dependencies into controller
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
        public async Task AddOrder_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Invalid model");
            var orderModel = new OrderModel();

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnNotFound_WhenRestaurantDoesNotExist()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 999999,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant)null);

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Nhà hàng không tồn tại.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task AddOrder_ShouldCreateFreeOrderSuccessfully()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 3,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            var orderId = 1;
            var orderEmail = new EmailForOrder
            {
                OrderId = "1",
                Name = "Test Restaurant",
                Email = "restaurant@example.com"
            };
            var restaurant = new Restaurant { Price = 0 };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _orderServiceMock.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });
            _orderServiceMock
                .Setup(service => service.GetEmailForOrderAsync(orderId, User_Role.RESTAURANT))
                .ReturnsAsync(orderEmail);

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldCreateOrderSuccessfully()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 3,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            var orderId = 1;
            var orderEmail = new EmailForOrder
            {
                OrderId = "1",
                Name = "Test Restaurant",
                Email = "restaurant@example.com"
            };
            var restaurant = new Restaurant { Price = 0 };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _orderServiceMock.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });
            _orderServiceMock
                .Setup(service => service.GetEmailForOrderAsync(orderId, User_Role.RESTAURANT))
                .ReturnsAsync(orderEmail);

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenOrderCreationFails()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 1,
                NameReceiver = "John Doe",
                PhoneReceiver = "1234567890",
                OrderMenus = new List<OrderMenuModelDto>
                {
                    new OrderMenuModelDto { MenuId = 1, Quantity = 2 }
                }
            };

            var restaurant = new Restaurant { Price = 100 };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _menuRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Menu { Price = 50 });
            _orderServiceMock.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync((Order)null); // Simulate failure

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo đơn hàng thất bại", badRequestResult.Value);
        }


        [TestMethod]
        public async Task AddOrder_Succeeds_WhenContentReservationIsNull()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 3,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                ContentReservation = null,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            var orderId = 1;
            var orderEmail = new EmailForOrder
            {
                OrderId = "1",
                Name = "Test Restaurant",
                Email = "restaurant@example.com"
            };
            var restaurant = new Restaurant { Price = 0 };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _orderServiceMock.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });
            _orderServiceMock
                .Setup(service => service.GetEmailForOrderAsync(orderId, User_Role.RESTAURANT))
                .ReturnsAsync(orderEmail);

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddOrder(orderModel) as OkObjectResult;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, result.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo đơn hàng thành công", result.Value);
        }
        [TestMethod]
        public async Task AddOrder_CreatesFreeOrder_WhenRestaurantPriceIsZeroAndNoMenus()
        {
            // Arrange
            var orderModel = new OrderModel
            {
                RestaurantId = 3,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                ContentReservation = null,
                TableIds = new List<int> { 1, 2, 3, }
            };

            var orderId = 1;
            var orderEmail = new EmailForOrder
            {
                OrderId = "1",
                Name = "Test Restaurant",
                Email = "restaurant@example.com"
            };
            var restaurant = new Restaurant { Price = 0 };
            _restaurantRepositoryMock.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _orderServiceMock.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });
            _orderServiceMock
                .Setup(service => service.GetEmailForOrderAsync(orderId, User_Role.RESTAURANT))
                .ReturnsAsync(orderEmail);

            _sendMailServiceMock
                .Setup(service => service.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo đơn hàng miễn phí thành công", okResult.Value);
        }
    }
}
