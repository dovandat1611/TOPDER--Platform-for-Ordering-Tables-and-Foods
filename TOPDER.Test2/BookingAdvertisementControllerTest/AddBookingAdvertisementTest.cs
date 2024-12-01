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
    public class AddBookingAdvertisementTest
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
        public async Task AddBookingAdvertisement_ShouldReturnOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var bookingAdvertisementDto = new CreateBookingAdvertisementDto
            {
                RestaurantId = 1,
                Title = "Special Offer",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(10)
            };

            _mockBookingAdvertisementService
                .Setup(service => service.AddAsync(It.IsAny<CreateBookingAdvertisementDto>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddBookingAdvertisement(bookingAdvertisementDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Booking advertisement created successfully.", okResult.Value);
        }

        [TestMethod]
        public async Task AddBookingAdvertisement_NullTitle_ReturnsInternalServerError()
        {
            // Arrange
            var invalidDto = new CreateBookingAdvertisementDto
            {
                RestaurantId = 1,
                Title = null, // Title is null
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(7)
            };

            _mockBookingAdvertisementService.Setup(service => service.AddAsync(invalidDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddBookingAdvertisement(invalidDto);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(serverErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while creating the booking advertisement.", serverErrorResult.Value);
        }

        [TestMethod]
        public async Task AddBookingAdvertisement_NullDates_ReturnsInternalServerError()
        {
            // Arrange
            var invalidDto = new CreateBookingAdvertisementDto
            {
                RestaurantId = 1,
                Title = "Special Offer",
                StartTime = default, // StartTime is null (default value for DateTime)
                EndTime = default // EndTime is null (default value for DateTime)
            };

            _mockBookingAdvertisementService.Setup(service => service.AddAsync(invalidDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddBookingAdvertisement(invalidDto);

            // Assert
            var serverErrorResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(serverErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while creating the booking advertisement.", serverErrorResult.Value);
        }

        

        [TestMethod]
        public async Task AddBookingAdvertisement_ShouldReturnInternalServerError_WhenCreationFails()
        {
            // Arrange
            var bookingAdvertisementDto = new CreateBookingAdvertisementDto
            {
                RestaurantId = 1,
                Title = "Special Offer",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(10)
            };

            _mockBookingAdvertisementService
                .Setup(service => service.AddAsync(It.IsAny<CreateBookingAdvertisementDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddBookingAdvertisement(bookingAdvertisementDto);

            // Assert
             Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, objectResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An error occurred while creating the booking advertisement.", objectResult.Value);
        }
    }
}
