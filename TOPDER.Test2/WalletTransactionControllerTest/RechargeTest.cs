using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using Microsoft.Extensions.Configuration;
using TOPDER.Service.Dtos.VNPAY;
using TOPDER.Repository.Entities;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]
    public class RechargeTests
    {
        private WalletTransactionController _controller;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private Mock<IConfiguration> _configurationMock;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _walletServiceMock = new Mock<IWalletService>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _configurationMock = new Mock<IConfiguration>();

            // Create controller instance with mock dependencies
            _controller = new WalletTransactionController(
                _walletTransactionServiceMock.Object,
                _walletServiceMock.Object,
                _paymentGatewayServiceMock.Object,
                _configurationMock.Object
            );
        }

        [TestMethod]
        public async Task Recharge_InvalidAmount_ReturnsBadRequest()
        {
            // Arrange
            var rechargeRequest = new RechargeWalletTransaction
            {
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 5000,  // Invalid amount (< 10000)
                PaymentGateway = PaymentGateway.VIETQR
            };

            // Act
            var result = await _controller.Recharge(rechargeRequest);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            var response = badRequestResult.Value as dynamic;
        }

        [TestMethod]
        public async Task Recharge_InvalidPaymentGateway_ReturnsBadRequest()
        {
            // Arrange
            var rechargeRequest = new RechargeWalletTransaction
            {
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 20000,
                PaymentGateway = "INVALID_PAYMENT_GATEWAY"  // Invalid payment gateway
            };

            // Act
            var result = await _controller.Recharge(rechargeRequest);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.               Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
            var response = badRequestResult.Value as dynamic;
        }

        [TestMethod]
        public async Task Recharge_SuccessWithVietQR_ReturnsOk()
        {
            // Arrange
            var rechargeRequest = new RechargeWalletTransaction
            {
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 20000,
                PaymentGateway = PaymentGateway.VIETQR
            };

            var walletTransactionDto = new WalletTransactionDto
            {
                TransactionId = 123,
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 20000,
                Status = Payment_Status.PENDING,
                TransactionType = Transaction_Type.RECHARGE,
                TransactionDate = DateTime.UtcNow,
                Description = Payment_Descriptions.RechargeDescription(20000)
            };

            var walletTransaction = new WalletTransaction
            {
                TransactionId = 123,
                WalletId = 1,
                TransactionAmount = 20000,
                Status = Payment_Status.PENDING,
                TransactionType = Transaction_Type.RECHARGE,
                TransactionDate = DateTime.UtcNow,
                Description = Payment_Descriptions.RechargeDescription(20000)
            };

            // Mock the service methods
            _walletTransactionServiceMock.Setup(s => s.AddRechargeAsync(It.IsAny<WalletTransactionDto>()))
                .ReturnsAsync(walletTransaction);

            // Mock the configuration values for CancelUrl and ReturnUrl
            _configurationMock.Setup(c => c["PayOSSettings:CancelUrl"]).Returns("https://cancelurl.com");
            _configurationMock.Setup(c => c["PayOSSettings:ReturnUrl"]).Returns("https://returnurl.com");

            // Mock the payment gateway service to return a URL for VietQR
            _paymentGatewayServiceMock.Setup(s => s.CreatePaymentUrlPayOS(It.IsAny<PaymentData>()))
                .ReturnsAsync(new CreatePaymentResult(
                    bin: "123456",
                    accountNumber: "9876543210",
                    amount: 20000,
                    description: "Recharge description",
                    orderCode: 1234567890L,
                    currency: "VND",
                    paymentLinkId: "abcd1234",
                    status: "Pending",
                    checkoutUrl: "https://somepaymentlink.com",
                    qrCode: "someQRCodeBase64"
                ));

            // Act
            var result = await _controller.Recharge(rechargeRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Recharge_SuccessWithVnPay_ReturnsOk()
        {
            // Arrange
            var rechargeRequest = new RechargeWalletTransaction
            {
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 30000,
                PaymentGateway = PaymentGateway.VNPAY
            };

            var walletTransactionDto = new WalletTransactionDto
            {
                TransactionId = 456,
                Uid = 1,
                WalletId = 1,
                TransactionAmount = 30000,
                Status = Payment_Status.PENDING
            };

            var walletTransaction = new WalletTransaction
            {
                TransactionId = 456,
                WalletId = 1,
                TransactionAmount = 30000,
                Status = Payment_Status.PENDING
            };

            // Mock the service methods
            _walletTransactionServiceMock.Setup(s => s.AddRechargeAsync(It.IsAny<WalletTransactionDto>()))
                .ReturnsAsync(walletTransaction);

            _paymentGatewayServiceMock.Setup(s => s.CreatePaymentUrlVnpay(It.IsAny<PaymentInformationModel>(), It.IsAny<HttpContext>()))
                .ReturnsAsync("https://vnpay-payment.com");

            // Act
            var result = await _controller.Recharge(rechargeRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

        }
    }
}
