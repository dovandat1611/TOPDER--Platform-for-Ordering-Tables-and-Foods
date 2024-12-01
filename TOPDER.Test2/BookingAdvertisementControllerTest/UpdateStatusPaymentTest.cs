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
    public class UpdateStatusPaymentTest
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
        public async Task UpdateStatusPayment_ValidBookingIdAndStatus_ReturnsOk()
        {
            // Arrange
            int bookingId = 1;
            string status = "Paid";
            BookingAdvertisementDto booking1 = new BookingAdvertisementDto
            {
                BookingId = 1,
                RestaurantId = 2,
                Title = "Dinner Booking 1",
                StartTime = new DateTime(2024, 12, 1, 18, 0, 0),
                EndTime = new DateTime(2024, 12, 1, 20, 0, 0),
                Status = "Confirmed",
                TotalAmount = 100.50m,
                StatusPayment = "Paid",
                CreatedAt = new DateTime(2024, 11, 28, 14, 30, 0)
            };
            _mockBookingAdvertisementService
                .Setup(s => s.UpdateStatusPaymentAsync(bookingId, status))
                .ReturnsAsync(booking1);

            // Act
            var result = await _controller.UpdateStatusPayment(bookingId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateStatusPayment_InvalidBookingIdOrStatus_ReturnsBadRequest()
        {
            // Arrange
            int bookingId = -1;
            string status = "InvalidStatus";
            BookingAdvertisementDto booking1 = new BookingAdvertisementDto
            {
                BookingId = 1,
                RestaurantId = 101,
                Title = "Dinner Booking 1",
                StartTime = new DateTime(2024, 12, 1, 18, 0, 0),
                EndTime = new DateTime(2024, 12, 1, 20, 0, 0),
                Status = "Confirmed",
                TotalAmount = 100.50m,
                StatusPayment = "Paid",
                CreatedAt = new DateTime(2024, 11, 28, 14, 30, 0)
            };
            _mockBookingAdvertisementService
                .Setup(s => s.UpdateStatusPaymentAsync(bookingId, status))
                .ReturnsAsync(booking1);

            // Act
            var result = await _controller.UpdateStatusPayment(bookingId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

    }
}
