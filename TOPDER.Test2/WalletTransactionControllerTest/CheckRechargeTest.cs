using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;
using Microsoft.Extensions.Configuration;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]
    public class CheckRechargeTest
    {
        private WalletTransactionController _controller;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IConfiguration> _configurationMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _walletServiceMock = new Mock<IWalletService>();
            _configurationMock = new Mock<IConfiguration>();

            _controller = new WalletTransactionController(
                _walletTransactionServiceMock.Object,
                _walletServiceMock.Object,
                null,
                _configurationMock.Object
            );
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsNullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;
            string status = null;

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsCancelled_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            int transactionId = 1;
            string status = Payment_Status.CANCELLED;

            _walletTransactionServiceMock
                .Setup(service => service.UpdateStatus(transactionId, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsCancelled_UpdateFails_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;
            string status = Payment_Status.CANCELLED;

            _walletTransactionServiceMock
                .Setup(service => service.UpdateStatus(transactionId, status))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsSuccessful_UpdateBalanceAndStatusSuccess_ReturnsOk()
        {
            // Arrange
            int transactionId = 1;
            string status = Payment_Status.SUCCESSFUL;

            var walletBalanceDto = new WalletBalanceDto
            {
                WalletId = 1,
                Uid = 1,
                WalletBalance = 1000
            };

            _walletTransactionServiceMock
                .Setup(service => service.GetWalletBalanceAsync(transactionId))
                .ReturnsAsync(walletBalanceDto);

            _walletServiceMock
                .Setup(service => service.UpdateWalletBalanceAsync(walletBalanceDto))
                .ReturnsAsync(true);

            _walletTransactionServiceMock
                .Setup(service => service.UpdateStatus(transactionId, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsSuccessful_UpdateBalanceFails_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;
            string status = Payment_Status.SUCCESSFUL;

            var walletBalanceDto = new WalletBalanceDto
            {
                WalletId = 1,
                Uid = 1,
                WalletBalance = 1000
            };

            _walletTransactionServiceMock
                .Setup(service => service.GetWalletBalanceAsync(transactionId))
                .ReturnsAsync(walletBalanceDto);

            _walletServiceMock
                .Setup(service => service.UpdateWalletBalanceAsync(walletBalanceDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckRecharge_StatusIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;
            string status = "INVALID";

            // Act
            var result = await _controller.CheckRecharge(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}
