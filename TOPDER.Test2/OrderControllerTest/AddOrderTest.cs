using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Email;
using TOPDER.Service.Dtos.Order;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.OrderControllerTest
{
    [TestClass]
    public class AddOrderTest
    {
        private Mock<IOrderService> _mockOrderService;
        private Mock<IOrderMenuService> _mockOrderMenuService;
        private Mock<IOrderTableService> _mockOrderTableService;
        private Mock<IWalletService> _mockWalletService;
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private Mock<IMenuRepository> _mockMenuRepository;
        private Mock<IRestaurantRepository> _mockRestaurantRepository;
        private Mock<IRestaurantService> _mockRestaurantService;
        private Mock<IUserService> _mockUserService;
        private Mock<IWalletTransactionService> _mockWalletTransactionService;
        private Mock<IPaymentGatewayService> _mockPaymentGatewayService;
        private Mock<ISendMailService> _mockSendMailService;
        private Mock<IDiscountMenuRepository> _mockDiscountMenuRepository;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;
        private Mock<IRestaurantPolicyService> _mockRestaurantPolicyService;
        private Mock<IOrderRepository> _mockOrderRepository;

        private OrderController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Mocking all services and repositories
            _mockOrderService = new Mock<IOrderService>();
            _mockOrderMenuService = new Mock<IOrderMenuService>();
            _mockOrderTableService = new Mock<IOrderTableService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _mockMenuRepository = new Mock<IMenuRepository>();
            _mockRestaurantRepository = new Mock<IRestaurantRepository>();
            _mockRestaurantService = new Mock<IRestaurantService>();
            _mockUserService = new Mock<IUserService>();
            _mockWalletTransactionService = new Mock<IWalletTransactionService>();
            _mockPaymentGatewayService = new Mock<IPaymentGatewayService>();
            _mockSendMailService = new Mock<ISendMailService>();
            _mockDiscountMenuRepository = new Mock<IDiscountMenuRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();
            _mockRestaurantPolicyService = new Mock<IRestaurantPolicyService>();
            _mockOrderRepository = new Mock<IOrderRepository>();

            // Initializing the controller with mocked dependencies
            _controller = new OrderController(
                _mockOrderService.Object,
                _mockOrderMenuService.Object,
                _mockWalletService.Object,
                _mockMenuRepository.Object,
                _mockRestaurantRepository.Object,
                _mockDiscountRepository.Object,
                _mockUserService.Object,
                _mockWalletTransactionService.Object,
                _mockPaymentGatewayService.Object,
                _mockSendMailService.Object,
                _mockOrderTableService.Object,
                _mockDiscountMenuRepository.Object,
                _mockConfiguration.Object,
                _mockRestaurantService.Object,
                _mockNotificationService.Object,
                _mockSignalRHub.Object,
                _mockRestaurantPolicyService.Object,
                _mockOrderRepository.Object
            );
        }


        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenNameReceiverIdIsNull()
        {
            // Arrange
            _controller.ModelState.AddModelError("NameReceiver", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = null,
                PhoneReceiver = "1234567890",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenPhoneReceiverIsIsNull()
        {
            // Arrange
            _controller.ModelState.AddModelError("PhoneReceiver", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = null,
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenTimeReservationIsIsNull()
        {
            // Arrange
            _controller.ModelState.AddModelError("TimeReservation", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "0373701816",
                DateReservation = DateTime.Now,
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenNumberPersonIsNegative()
        {
            // Arrange
            _controller.ModelState.AddModelError("NumberPerson", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "0373701816",
                DateReservation = DateTime.Now,
                NumberPerson = -1,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenNumberChildIsNegative()
        {
            // Arrange
            _controller.ModelState.AddModelError("NumberChild", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "0373701816",
                DateReservation = DateTime.Now,
                NumberPerson = 2,
                NumberChild = -1,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenPhoneReceiverInValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("PhoneReceiver", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234",
                DateReservation = DateTime.Now,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldReturnBadRequest_WhenDateReservationIsPass()
        {
            // Arrange
            _controller.ModelState.AddModelError("DateReservation", "Invalid model");
            var orderModel = new OrderModel
            {
                RestaurantId = 1,
                CustomerId = 2,
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                 DateReservation  = new DateTime(2024, 1, 5, 10, 30, 0) ,
                TimeReservation = TimeSpan.FromHours(19),
                NumberPerson = 4,
                NumberChild = 3,
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } },
                TableIds = new List<int> { 1, 2, 3, }
            };

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
                RestaurantId = -1,
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
            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Restaurant)null);

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
                RestaurantId = 1,
                CustomerId = 1  ,
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
            var mockRestaurant = new Restaurant { Uid = 1, Price = 100, Discount = 10 };
            // Mock các phương thức cần thiết
            _mockRestaurantRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockRestaurant);
            _mockOrderService.Setup(os => os.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });

            // Mock phương thức GetEmailForOrderAsync để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetEmailForOrderAsync(It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(new EmailForOrder { Email = "test@example.com" });

            // Mock phương thức GetOrderPaid để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetOrderPaid(It.IsAny<int>()))
                             .ReturnsAsync(new OrderPaidEmail { OrderId = "1", TotalAmount = 100 });

            _mockOrderTableService.Setup(ots => ots.AddRangeAsync(It.IsAny<CreateRestaurantOrderTablesDto>())).ReturnsAsync(true);

            // Mock phương thức SendEmailAsync
            _mockSendMailService.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            var mockRestaurant = new Restaurant { Uid = 1, Price = 100, Discount = 10 };
            // Mock các phương thức cần thiết
            _mockRestaurantRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockRestaurant);
            _mockOrderService.Setup(os => os.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });

            // Mock phương thức GetEmailForOrderAsync để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetEmailForOrderAsync(It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(new EmailForOrder { Email = "test@example.com" });

            // Mock phương thức GetOrderPaid để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetOrderPaid(It.IsAny<int>()))
                             .ReturnsAsync(new OrderPaidEmail { OrderId = "1", TotalAmount = 100 });

            _mockOrderTableService.Setup(ots => ots.AddRangeAsync(It.IsAny<CreateRestaurantOrderTablesDto>())).ReturnsAsync(true);

            // Mock phương thức SendEmailAsync
            _mockSendMailService.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);
            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task AddOrder_ShouldCreateOrderSuccessfully_WhenNumberChildIsZero()
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
                NumberChild = 0,
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
            var mockRestaurant = new Restaurant { Uid = 1, Price = 100, Discount = 10 };
            // Mock các phương thức cần thiết
            _mockRestaurantRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockRestaurant);
            _mockOrderService.Setup(os => os.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });

            // Mock phương thức GetEmailForOrderAsync để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetEmailForOrderAsync(It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(new EmailForOrder { Email = "test@example.com" });

            // Mock phương thức GetOrderPaid để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetOrderPaid(It.IsAny<int>()))
                             .ReturnsAsync(new OrderPaidEmail { OrderId = "1", TotalAmount = 100 });

            _mockOrderTableService.Setup(ots => ots.AddRangeAsync(It.IsAny<CreateRestaurantOrderTablesDto>())).ReturnsAsync(true);

            // Mock phương thức SendEmailAsync
            _mockSendMailService.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
                NameReceiver = "Đỗ Văn Đạt",
                PhoneReceiver = "1234567890",
                OrderMenus = new List<OrderMenuModelDto>
                {
                    new OrderMenuModelDto { MenuId = 1, Quantity = 2 }
                }
            };

            var restaurant = new Restaurant { Price = 100 };
            _mockRestaurantRepository.Setup(repo => repo.GetByIdAsync(orderModel.RestaurantId)).ReturnsAsync(restaurant);
            _mockMenuRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Menu { Price = 50 });
            _mockOrderService.Setup(service => service.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync((Order)null); // Simulate failure

            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo đơn hàng thất bại", badRequestResult.Value);
        }


        [TestMethod]
        public async Task AddOrder_ShouldCreateOrderAndSendEmail_WhenDataIsValid()
        {
            // Arrange
            var mockRestaurant = new Restaurant { Uid = 1, Price = 100, Discount = 10 };
            var mockOrderModel = new OrderModel
            {
                CustomerId = 1,
                RestaurantId = 1,
                NameReceiver = "John",
                PhoneReceiver = "123456789",
                TimeReservation = TimeSpan.FromHours(19),
                DateReservation = DateTime.Now.AddDays(1),
                NumberPerson = 2,
                ContentReservation = "Test",
                OrderMenus = new List<OrderMenuModelDto> { new OrderMenuModelDto { MenuId = 1, Quantity = 2 } }
            };

            // Mock các phương thức cần thiết
            _mockRestaurantRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockRestaurant);
            _mockOrderService.Setup(os => os.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });

            // Mock phương thức GetEmailForOrderAsync để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetEmailForOrderAsync(It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(new EmailForOrder { Email = "test@example.com" });

            // Mock phương thức GetOrderPaid để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetOrderPaid(It.IsAny<int>()))
                             .ReturnsAsync(new OrderPaidEmail { OrderId = "1", TotalAmount = 100 });

            _mockOrderTableService.Setup(ots => ots.AddRangeAsync(It.IsAny<CreateRestaurantOrderTablesDto>())).ReturnsAsync(true);

            // Mock phương thức SendEmailAsync
            _mockSendMailService.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddOrder(mockOrderModel);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockOrderService.Verify(os => os.AddAsync(It.IsAny<OrderDto>()), Times.Once);
            _mockSendMailService.Verify(sms => sms.SendEmailAsync("test@example.com", Email_Subject.NEWORDER, It.IsAny<string>()), Times.Once);
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
                TableIds = new List<int> { 1, 2, 3 }
            };

            var orderId = 1;
            var orderEmail = new EmailForOrder
            {
                OrderId = "1",
                Name = "Test Restaurant",
                Email = "restaurant@example.com"
            };
            var mockRestaurant = new Restaurant { Uid = 1, Price = 100, Discount = 10 };
            // Mock các phương thức cần thiết
            _mockRestaurantRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(mockRestaurant);
            _mockOrderService.Setup(os => os.AddAsync(It.IsAny<OrderDto>())).ReturnsAsync(new Order { OrderId = 1 });

            // Mock phương thức GetEmailForOrderAsync để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetEmailForOrderAsync(It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(new EmailForOrder { Email = "test@example.com" });

            // Mock phương thức GetOrderPaid để tránh NullReferenceException
            _mockOrderService.Setup(os => os.GetOrderPaid(It.IsAny<int>()))
                             .ReturnsAsync(new OrderPaidEmail { OrderId = "1", TotalAmount = 100 });

            _mockOrderTableService.Setup(ots => ots.AddRangeAsync(It.IsAny<CreateRestaurantOrderTablesDto>())).ReturnsAsync(true);

            // Mock phương thức SendEmailAsync
            _mockSendMailService.Setup(sms => sms.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.CompletedTask);


            // Act
            var result = await _controller.AddOrder(orderModel);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Tạo đơn hàng thành công", okResult.Value);
        }
    }
}
