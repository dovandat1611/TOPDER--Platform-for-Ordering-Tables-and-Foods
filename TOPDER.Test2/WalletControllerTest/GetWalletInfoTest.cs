using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Wallet;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.WalletControllerTest
{
    [TestClass]
    public class GetWalletInfoTest
    {
        private Mock<IWalletService> _walletServiceMock;
        private WalletController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mock service and controller
            _walletServiceMock = new Mock<IWalletService>();
            _controller = new WalletController(_walletServiceMock.Object);
        }

        [TestMethod]
        public async Task GetWalletInfo_Success_ReturnsOk()
        {
            // Arrange
            int uid = 1; // Example User ID
            var walletDto = new WalletDto
            {
                WalletId = 1,
                Uid = uid,
                WalletBalance = 1000,
                BankCode = "Test Bank",
                OtpCode = "123456"
            };
            _walletServiceMock
                .Setup(service => service.GetInforWalletAsync(uid))
                .ReturnsAsync(walletDto);

            // Act
            var result = await _controller.GetWalletInfo(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // Ensure status code is 200
            var returnedWallet = okResult.Value as WalletDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedWallet); // Ensure the returned value is not null
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(walletDto.WalletId, returnedWallet.WalletId); // Verify wallet data
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(walletDto.WalletBalance, returnedWallet.WalletBalance);
        }

        [TestMethod]
        public async Task GetWalletInfo_NotFound_ReturnsNotFound()
        {
            // Arrange
            int uid = -1; // Example User ID
            _walletServiceMock
                .Setup(service => service.GetInforWalletAsync(uid))
                .ReturnsAsync((WalletDto)null);

            // Act
            var result = await _controller.GetWalletInfo(uid);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult); // Ensure the result is NotFoundObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode); // Ensure status code is 404
        }

        [TestMethod]
        public async Task GetWalletInfo_Exception_ReturnsInternalServerError()
        {
            // Arrange
            int uid = 1; // Example User ID
            var exceptionMessage = "Database connection error";
            _walletServiceMock
                .Setup(service => service.GetInforWalletAsync(uid))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetWalletInfo(uid);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(statusCodeResult); // Ensure the result is ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, statusCodeResult.StatusCode); // Ensure status code is 500
        }
    }

}
