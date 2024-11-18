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
    public class CheckExistOTPTest
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
        public async Task CheckExistOTP_Exists_ReturnsOk()
        {
            // Arrange
            int uid = 101; // Example User ID
            _walletServiceMock
                .Setup(service => service.CheckExistOTP(uid))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckExistOTP(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is OkObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode); // Ensure status code is 200
        }

        [TestMethod]
        public async Task CheckExistOTP_NotExists_ReturnsNotFound()
        {
            // Arrange
            int uid = 102; // Example User ID
            _walletServiceMock
                .Setup(service => service.CheckExistOTP(uid))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CheckExistOTP(uid);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult); // Ensure the result is NotFoundObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode); // Ensure status code is 404
        }
    }

}
