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
    public class UpdateWalletBankTest
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
        public async Task UpdateWalletBank_ValidDto_ReturnsOk()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 1,
                Uid = 101,
                BankCode = "VCB",
                AccountNo = "123456789",
                AccountName = "Nguyen Van A"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult, "Result should be of type OkObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode, "Status code should be 200.");
        }

        [TestMethod]
        public async Task UpdateWalletBank_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 1,
                Uid = 101,
                BankCode = null, // Invalid bank code
                AccountNo = "123456789",
                AccountName = "Nguyen Van A"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }
        [TestMethod]
        public async Task UpdateWalletBank_BankCodeNull_ReturnsBadRequest()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 1,
                Uid = 1,
                BankCode = null, // Null field
                AccountNo = "123456789",
                AccountName = "Test User"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(false); // Giả lập cập nhật thất bại

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }
        [TestMethod]
        public async Task UpdateWalletBank_AccountNoNull_ReturnsBadRequest()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 1,
                Uid = 1,
                BankCode = "ABC123",
                AccountNo = null, // Null field
                AccountName = "Test User"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(false); // Giả lập cập nhật thất bại

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }
        [TestMethod]
        public async Task UpdateWalletBank_UidDefault_ReturnsBadRequest()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 1,
                Uid = 0, // Default value for int
                BankCode = "ABC123",
                AccountNo = "123456789",
                AccountName = "Test User"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(false); // Giả lập cập nhật thất bại

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }
        [TestMethod]
        public async Task UpdateWalletBank_WalletIdDefault_ReturnsBadRequest()
        {
            // Arrange
            var walletBankDto = new WalletBankDto
            {
                WalletId = 0, // Default value for int
                Uid = 1,
                BankCode = "ABC123",
                AccountNo = "123456789",
                AccountName = "Test User"
            };

            _walletServiceMock
                .Setup(service => service.UpdateWalletBankAsync(walletBankDto))
                .ReturnsAsync(false); // Giả lập cập nhật thất bại

            // Act
            var result = await _controller.UpdateWalletBank(walletBankDto);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }


    }
}
