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
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]

    public class GetPagingTest
    {
        private WalletTransactionController _controller;
        private Mock<IWalletTransactionService> _walletTransactionServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<IPaymentGatewayService> _paymentGatewayServiceMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IHubContext<AppHub>> _signalRHubMock;
        private Mock<INotificationService> _notificationServiceMock;

        [TestInitialize]
        public void TestInitialize()
        {
            // Khởi tạo các mock objects
            _walletTransactionServiceMock = new Mock<IWalletTransactionService>();
            _walletServiceMock = new Mock<IWalletService>();
            _paymentGatewayServiceMock = new Mock<IPaymentGatewayService>();
            _configurationMock = new Mock<IConfiguration>();
            _signalRHubMock = new Mock<IHubContext<AppHub>>();
            _notificationServiceMock = new Mock<INotificationService>();

            // Khởi tạo WalletTransactionController với các mock objects
            _controller = new WalletTransactionController(
                _walletTransactionServiceMock.Object,
                _walletServiceMock.Object,
                _paymentGatewayServiceMock.Object,
                _configurationMock.Object,
                _signalRHubMock.Object,
                _notificationServiceMock.Object
            );
        }


        [TestMethod]
        public async Task GetPaging_Success_ReturnsOk()
        {
            // Arrange
            string status = "Withdraw";  // Example status filter
            var walletTransactionHistory = new List<WalletTransactionDto>
            {
                new WalletTransactionDto
                {
                    TransactionId = 1,
                    Uid = 1,
                    WalletId = 1,
                    TransactionAmount = 10000,
                    TransactionDate = DateTime.UtcNow,
                    Status = "Withdraw"
                },
                new WalletTransactionDto
                {
                    TransactionId = 2,
                    Uid = 2,
                    WalletId = 2,
                    TransactionAmount = 20000,
                    TransactionDate = DateTime.UtcNow,
                    Status = "Withdraw"
                }
            };
            

            // Act
            var result = await _controller.GetPaging(status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

        }

        [TestMethod]
        public async Task GetPaging_NoStatus_ReturnsOk()
        {
            // Arrange
            string? status = null;  // Test with no status filter
            var walletTransactionHistory = new List<WalletTransactionDto>
    {
        new WalletTransactionDto
        {
            TransactionId = 1,
            Uid = 1,
            WalletId = 1,
            TransactionAmount = 10000,
            TransactionDate = DateTime.UtcNow,
            Status = "Withdraw"
        },
        new WalletTransactionDto
        {
            TransactionId = 2,
            Uid = 2,
            WalletId = 2,
            TransactionAmount = 20000,
            TransactionDate = DateTime.UtcNow,
            Status = "Pending"
        }
    };

            // Act
            var result = await _controller.GetPaging(status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(200, okResult.StatusCode);

        }

    }
}
