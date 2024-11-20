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
using TOPDER.Service.Dtos.BookingAdvertisement;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.BookingAdvertisementControllerTest
{
    [TestClass]
    public class GetAllBookingAdvertisementAvailableTest
    {
        private BookingAdvertisementController _controller;
        private Mock<IBookingAdvertisementService> _mockBookingAdvertisementService;
        private Mock<IBookingAdvertisementRepository> _mockBookingAdvertisementRepository;
        private Mock<IWalletTransactionService> _mockWalletTransactionService;
        private Mock<IWalletService> _mockWalletService;
        private Mock<IUserService> _mockUserService;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IPaymentGatewayService> _mockPaymentGatewayService;

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

            // Creating the controller instance with the mocked dependencies
            _controller = new BookingAdvertisementController(
                _mockBookingAdvertisementService.Object,
                _mockBookingAdvertisementRepository.Object,
                _mockWalletTransactionService.Object,
                _mockWalletService.Object,
                _mockUserService.Object,
                _mockConfiguration.Object,
                _mockPaymentGatewayService.Object);
        }

        [TestMethod]
        public async Task GetAllBookingAdvertisementAvailable_ReturnsOkResultWithBookingAdvertisements()
        {
            // Arrange
            var expectedAdvertisements = new List<BookingAdvertisementViewDto>
            {
                new BookingAdvertisementViewDto
                {
                    BookingId = 1,
                    Uid = 123,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    Title = "Ad Title 1",
                    CategoryRestaurantId = 10,
                    CategoryName = "Category 1"
                },
                new BookingAdvertisementViewDto
                {
                    BookingId = 2,
                    Uid = 456,
                    Logo = "logo2.png",
                    NameRes = "Restaurant 2",
                    Title = "Ad Title 2",
                    CategoryRestaurantId = 20,
                    CategoryName = "Category 2"
                }
            };

            _mockBookingAdvertisementService
                .Setup(service => service.GetAllBookingAdvertisementAvailableAsync())
                .ReturnsAsync(expectedAdvertisements);

            // Act
            var result = await _controller.GetAllBookingAdvertisementAvailable();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var bookingAdvertisements = okResult.Value as List<BookingAdvertisementViewDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(bookingAdvertisements);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedAdvertisements.Count, bookingAdvertisements.Count);

        }


    }
}
