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

namespace TOPDER.Test2.WalletControllerTest
{
    [TestClass]
    public class CheckOTPTest
    {
        private Mock<IWalletService> _walletServiceMock;
        private WalletController _walletController;

        [TestInitialize]
        public void Setup()
        {
            _walletServiceMock = new Mock<IWalletService>();
            _walletController = new WalletController(_walletServiceMock.Object);
        }

        [TestMethod]
        public async Task CheckExistOTP_Exists_ReturnsOk()
        {
            // Arrange
            int uid = 101; // Example User ID
            _walletServiceMock
                .Setup(service => service.CheckExistOTP(uid))
                .ReturnsAsync(true);

            // Act
            var result = await _walletController.CheckExistOTP(uid);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult, "Result should be of type OkObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode, "Status code should be 200.");
        }


        [TestMethod]
        public async Task CheckOTP_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            int uid = 1;
            string otp = "654321";
            _walletServiceMock.Setup(service => service.CheckOTP(uid, otp))
                              .ReturnsAsync(false);

            // Act
            var result = await _walletController.CheckOTP(uid, otp);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result, "Result should not be null.");
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult, "Result should be of type BadRequestObjectResult.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode, "Status code should be 400.");
        }

    }
}
