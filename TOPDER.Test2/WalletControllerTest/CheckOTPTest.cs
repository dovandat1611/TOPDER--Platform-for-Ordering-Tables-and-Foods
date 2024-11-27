using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TOPDER.Test2.WalletControllerTest
{
    [TestClass]
    public class CheckOTPTest
    {
        private Mock<IWalletService> _walletServiceMock;
        private WalletController _controller;

        [TestInitialize]
        public void Setup()
        {
            _walletServiceMock = new Mock<IWalletService>();
            _controller = new WalletController(_walletServiceMock.Object);
        }

        [TestMethod]
        public async Task CheckOTP_ValidOTP_ReturnsOk()
        {
            // Arrange
            int uid = 1;
            string otp = "123456";
            _walletServiceMock
                .Setup(s => s.CheckOTP(uid, otp))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckOTP(uid, otp);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckOTP_InvalidOTP_ReturnsBadRequest()
        {
            // Arrange
            int uid = 1;
            string otp = "invalid";
            _walletServiceMock
                .Setup(s => s.CheckOTP(uid, otp))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CheckOTP(uid, otp);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}
