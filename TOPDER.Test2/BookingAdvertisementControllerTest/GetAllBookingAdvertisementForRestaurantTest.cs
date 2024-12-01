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
using TOPDER.Service.Dtos.BookingAdvertisement;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BookingAdvertisementControllerTest
{
    [TestClass]
    public class GetAllBookingAdvertisementForRestaurantTest
    {
        private BookingAdvertisementController _controller;
        private Mock<IBookingAdvertisementService> _mockBookingAdvertisementService;
        private Mock<IBookingAdvertisementRepository> _mockBookingAdvertisementRepository;
        private Mock<IWalletTransactionService> _mockWalletTransactionService;
        private Mock<IWalletService> _mockWalletService;
        private Mock<IUserService> _mockUserService;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IPaymentGatewayService> _mockPaymentGatewayService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;

        [TestInitialize]
        public void Initialize()
        {
            // Mocking the dependencies
            _mockBookingAdvertisementService = new Mock<IBookingAdvertisementService>();
            _mockBookingAdvertisementRepository = new Mock<IBookingAdvertisementRepository>();
            _mockWalletTransactionService = new Mock<IWalletTransactionService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockPaymentGatewayService = new Mock<IPaymentGatewayService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();

            // Creating the controller instance with the mocked dependencies
            _controller = new BookingAdvertisementController(
                _mockBookingAdvertisementService.Object,
                _mockBookingAdvertisementRepository.Object,
                _mockWalletTransactionService.Object,
                _mockWalletService.Object,
                _mockUserService.Object,
                _mockConfiguration.Object,
                _mockPaymentGatewayService.Object,
                _mockNotificationService.Object,
                _mockSignalRHub.Object);
        }


        [TestMethod]
        public async Task GetAllBookingAdvertisementForRestaurant_WithValidRestaurantId_ReturnsBookingAdvertisements()
        {
            // Arrange
            int restaurantId = 1;
            var mockBookingAdvertisements = new List<BookingAdvertisementDto>
        {
            new BookingAdvertisementDto
            {
                BookingId = 1,
                RestaurantId = restaurantId,
                Title = "Advertisement 1",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                Status = "Active",
                TotalAmount = 1000m,
                StatusPayment = "Paid",
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new BookingAdvertisementDto
            {
                BookingId = 2,
                RestaurantId = restaurantId,
                Title = "Advertisement 2",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(2),
                Status = "Pending",
                TotalAmount = 2000m,
                StatusPayment = "Unpaid",
                CreatedAt = DateTime.Now.AddDays(-2)
            }
        };

            _mockBookingAdvertisementService
                .Setup(service => service.GetAllBookingAdvertisementForRestaurantAsync(restaurantId))
                .ReturnsAsync(mockBookingAdvertisements);

            // Act
            var result = await _controller.GetAllBookingAdvertisementForRestaurant(restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var bookingAdvertisements = okResult.Value as List<BookingAdvertisementDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(bookingAdvertisements);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, bookingAdvertisements.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(restaurantId, bookingAdvertisements[0].RestaurantId);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Advertisement 1", bookingAdvertisements[0].Title);
        }

        [TestMethod]
        public async Task GetAllBookingAdvertisementForRestaurant_WithInvalidRestaurantId_ReturnsEmptyList()
        {
            // Arrange
            int restaurantId = -1;
            _mockBookingAdvertisementService
                .Setup(service => service.GetAllBookingAdvertisementForRestaurantAsync(restaurantId))
                .ReturnsAsync(new List<BookingAdvertisementDto>());

            // Act
            var result = await _controller.GetAllBookingAdvertisementForRestaurant(restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var bookingAdvertisements = okResult.Value as List<BookingAdvertisementDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(bookingAdvertisements);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, bookingAdvertisements.Count);
        }
    }
}
