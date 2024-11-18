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
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]
    public class WithdrawTest
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
        public async Task Withdraw_InvalidBalance_ReturnsBadRequest()
        {
            // Arrange
            var walletTransactionWithDrawDto = new WalletTransactionWithDrawDto
            {
                Uid = 1,
                WalletId = 101,
                TransactionAmount = 1000
            };

            _walletServiceMock
                .Setup(service => service.GetBalanceAsync(walletTransactionWithDrawDto.WalletId, walletTransactionWithDrawDto.Uid))
                .ReturnsAsync(0); // Invalid balance

            // Act
            var result = await _controller.Withdraw(walletTransactionWithDrawDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Withdraw_InsufficientBalance_ReturnsBadRequest()
        {
            // Arrange
            var walletTransactionWithDrawDto = new WalletTransactionWithDrawDto
            {
                Uid = 1,
                WalletId = 101,
                TransactionAmount = 1000
            };

            _walletServiceMock
                .Setup(service => service.GetBalanceAsync(walletTransactionWithDrawDto.WalletId, walletTransactionWithDrawDto.Uid))
                .ReturnsAsync(500); // Insufficient balance

            // Act
            var result = await _controller.Withdraw(walletTransactionWithDrawDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Withdraw_UpdateBalanceFails_ReturnsBadRequest()
        {
            // Arrange
            var walletTransactionWithDrawDto = new WalletTransactionWithDrawDto
            {
                Uid = 1,
                WalletId = 101,
                TransactionAmount = 500
            };

            _walletServiceMock
                .Setup(service => service.GetBalanceAsync(walletTransactionWithDrawDto.WalletId, walletTransactionWithDrawDto.Uid))
                .ReturnsAsync(1000); // Valid balance

            _walletServiceMock
                .Setup(service => service.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                .ReturnsAsync(false); // Update balance fails

            // Act
            var result = await _controller.Withdraw(walletTransactionWithDrawDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Withdraw_TransactionAndUpdateSuccess_ReturnsOk()
        {
            // Arrange
            var walletTransactionWithDrawDto = new WalletTransactionWithDrawDto
            {
                Uid = 1,
                WalletId = 101,
                TransactionAmount = 500
            };

            _walletServiceMock
                .Setup(service => service.GetBalanceAsync(walletTransactionWithDrawDto.WalletId, walletTransactionWithDrawDto.Uid))
                .ReturnsAsync(1000); // Valid balance

            _walletServiceMock
                .Setup(service => service.UpdateWalletBalanceAsync(It.IsAny<WalletBalanceDto>()))
                .ReturnsAsync(true); // Update balance succeeds

            _walletTransactionServiceMock
                .Setup(service => service.AddAsync(It.IsAny<WalletTransactionDto>()))
                .ReturnsAsync(true); // Transaction succeeds

            // Act
            var result = await _controller.Withdraw(walletTransactionWithDrawDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
