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
    public class GetAllBookingAdvertisementForAdminTest
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
        public async Task GetAllBookingAdvertisementForAdmin_ReturnsOkResultWithBookingAdvertisements()
        {
            // Arrange
            var expectedAdvertisements = new List<BookingAdvertisementAdminDto>
            {
                new BookingAdvertisementAdminDto
                {
                    BookingId = 1,
                    RestaurantId = 101,
                    RestaurantName = "Gourmet Restaurant",
                    RestaurantImage = "image1.jpg",
                    Title = "Grand Opening Special",
                    StartTime = DateTime.UtcNow.AddDays(-10),
                    EndTime = DateTime.UtcNow.AddDays(10),
                    Status = "Active",
                    TotalAmount = 1200.00m,
                    StatusPayment = "Paid",
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new BookingAdvertisementAdminDto
                {
                    BookingId = 2,
                    RestaurantId = 202,
                    RestaurantName = "Bistro Cafe",
                    RestaurantImage = "image2.jpg",
                    Title = "Weekend Discounts",
                    StartTime = DateTime.UtcNow.AddDays(-20),
                    EndTime = DateTime.UtcNow.AddDays(-5),
                    Status = "Expired",
                    TotalAmount = 800.00m,
                    StatusPayment = "Unpaid",
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                }
            };

            _mockBookingAdvertisementService
                .Setup(service => service.GetAllBookingAdvertisementForAdminAsync())
                .ReturnsAsync(expectedAdvertisements);

            // Act
            var result = await _controller.GetAllBookingAdvertisementForAdmin();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var bookingAdvertisements = okResult.Value as List<BookingAdvertisementAdminDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(bookingAdvertisements);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedAdvertisements.Count, bookingAdvertisements.Count);

            
        }
    }
}
