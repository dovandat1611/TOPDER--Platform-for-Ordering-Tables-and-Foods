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
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.BookingAdvertisementControllerTest
{
    [TestClass]
    public class ChoosePaymentGatePaymentGatewayTest
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
        public async Task ChoosePaymentGatePaymentGateway_ISBALANCE_SuccessfulPayment_ReturnsOk()
        {
            // Arrange
            int bookingId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            var bookingAdvertisement = new BookingAdvertisement
            {
                BookingId = bookingId,
                RestaurantId = 1,
                TotalAmount = 100
            };

            _mockBookingAdvertisementRepository.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync(bookingAdvertisement);
            _mockWalletService.Setup(service => service.GetBalanceOrderAsync(1)).ReturnsAsync(200);
            _mockUserService.Setup(service => service.GetInformationUserOrderIsBalance(1)).ReturnsAsync(new UserOrderIsBalance
            {
                WalletId = 1,
                Id = 1
            });
            _mockWalletTransactionService.Setup(service => service.AddAsync(It.IsAny<WalletTransactionDto>())).ReturnsAsync(true);
            _mockWalletService.Setup(service => service.UpdateWalletBalanceOrderAsync(It.IsAny<WalletBalanceOrderDto>())).ReturnsAsync(true);
            _mockBookingAdvertisementService.Setup(service => service.UpdateStatusPaymentAsync(bookingId, Payment_Status.SUCCESSFUL)).ReturnsAsync(true);

            // Act
            var result = await _controller.ChoosePaymentGatePaymentGateway(bookingId, paymentGateway);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thanh toán booking thành công", okResult.Value);
        }

        [TestMethod]
        public async Task ChoosePaymentGatePaymentGateway_ISBALANCE_InsufficientBalance_ReturnsBadRequest()
        {
            // Arrange
            int bookingId = 1;
            string paymentGateway = PaymentGateway.ISBALANCE;
            var bookingAdvertisement = new BookingAdvertisement
            {
                BookingId = bookingId,
                RestaurantId = 1,
                TotalAmount = 300
            };

            _mockBookingAdvertisementRepository.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync(bookingAdvertisement);
            _mockWalletService.Setup(service => service.GetBalanceOrderAsync(1)).ReturnsAsync(200);

            // Act
            var result = await _controller.ChoosePaymentGatePaymentGateway(bookingId, paymentGateway);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Số dư ví không đủ cho giao dịch này.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChoosePaymentGatePaymentGateway_InvalidPaymentGateway_ReturnsBadRequest()
        {
            // Arrange
            int bookingId = 1;
            string paymentGateway = "INVALID_GATEWAY";

            _mockBookingAdvertisementRepository.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync(new BookingAdvertisement
            {
                BookingId = bookingId
            });

            // Act
            var result = await _controller.ChoosePaymentGatePaymentGateway(bookingId, paymentGateway);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cổng thanh toán không hợp lệ hoặc không tìm thấy booking.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task ChoosePaymentGatePaymentGateway_BookingNotFound_ReturnsBadRequest()
        {
            // Arrange
            int bookingId = 999;
            string paymentGateway = PaymentGateway.ISBALANCE;

            _mockBookingAdvertisementRepository.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync((BookingAdvertisement)null);

            // Act
            var result = await _controller.ChoosePaymentGatePaymentGateway(bookingId, paymentGateway);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cổng thanh toán không hợp lệ hoặc không tìm thấy booking.", badRequestResult.Value);
        }

    }
}
