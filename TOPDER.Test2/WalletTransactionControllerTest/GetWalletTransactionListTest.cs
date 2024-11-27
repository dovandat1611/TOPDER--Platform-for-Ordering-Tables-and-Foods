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
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]
    public class GetWalletTransactionListTest
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
        public async Task GetWalletTransactionList_Success_ReturnsOk()
        {
            // Arrange
            int uid = 1;
            var walletTransactionHistory = new List<WalletTransactionDto>
    {
        new WalletTransactionDto
        {
            TransactionId = 1,
            Uid = 1,
            WalletId = 1,
            TransactionAmount = 10000,
            TransactionDate = DateTime.UtcNow,
            Status = Payment_Status.PENDING
        },
        new WalletTransactionDto
        {
            TransactionId = 2,
            Uid = 1,
            WalletId = 1,
            TransactionAmount = 20000,
            TransactionDate = DateTime.UtcNow,
            Status = Payment_Status.SUCCESSFUL
        }
    };

            // Mock the service method
            _walletTransactionServiceMock.Setup(s => s.GetWalletTransactionHistoryAsync(uid))
                .ReturnsAsync(walletTransactionHistory);

            // Act
            var result = await _controller.GetWalletTransactionList(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as List<WalletTransactionDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, response.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10000, response[0].TransactionAmount);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(20000, response[1].TransactionAmount);
        }

        [TestMethod]
        public async Task GetWalletTransactionList_Exception_ReturnsInternalServerError()
        {
            // Arrange
            int uid = -1;
            var exceptionMessage = "Test exception";

            // Mock the service method to throw an exception
            _walletTransactionServiceMock.Setup(s => s.GetWalletTransactionHistoryAsync(uid))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetWalletTransactionList(uid);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult.StatusCode);

        }

    }
}
