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
    public class AddOTPTest
    {
        private Mock<IWalletService> _walletServiceMock;
        private WalletController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the mock service and controller
            _walletServiceMock = new Mock<IWalletService>();
            _controller = new WalletController(_walletServiceMock.Object);
        }

        [TestMethod]
        public async Task AddOTP_Success_ReturnsOk()
        {
            // Arrange
            var walletOtpDto = new WalletOtpDto
            {
                WalletId = 1,
                Uid = 101,
                OtpCode = "123456"
            };

            _walletServiceMock
                .Setup(service => service.AddOTPAsync(walletOtpDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddOTP(walletOtpDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode); // Ensure status code is 200
            var message = okResult.Value as dynamic;
        }

        [TestMethod]
        public async Task AddOTP_Failure_ReturnsBadRequest()
        {
            // Arrange
            var walletOtpDto = new WalletOtpDto
            {
                WalletId = 2,
                Uid = 202,
                OtpCode = "654321"
            };

            _walletServiceMock
                .Setup(service => service.AddOTPAsync(walletOtpDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddOTP(walletOtpDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult); // Ensure the result is BadRequestObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // Ensure status code is 400
        }
    }

}
