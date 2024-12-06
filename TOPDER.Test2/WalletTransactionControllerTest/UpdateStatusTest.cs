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
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.WalletTransactionControllerTest
{
    [TestClass]
    public class UpdateStatusTest
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
        public async Task UpdateStatus_ValidStatus_ReturnsOk()
        {
            // Arrange
            int transactionId = 1;
            string status = Payment_Status.SUCCESSFUL;

            // Mock the service method to return true (indicating a successful status update)
            _walletTransactionServiceMock.Setup(s => s.UpdateStatus(transactionId, status))
                .ReturnsAsync(true);
            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _signalRHubMock.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);

            // Act
            var result = await _controller.UpdateStatus(transactionId, status);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }


        [TestMethod]
        public async Task UpdateStatus_InvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;
            string status = "INVALID_STATUS"; // This status is not valid

            // Act
            var result = await _controller.UpdateStatus(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateStatus_ServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = -1;
            string status = Payment_Status.SUCCESSFUL;

            // Mock the service method to return false (indicating a failed status update)
            _walletTransactionServiceMock.Setup(s => s.UpdateStatus(transactionId, status))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateStatus(transactionId, status);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}
