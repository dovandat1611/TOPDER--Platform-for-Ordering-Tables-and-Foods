using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.ReportControllerTest
{
    [TestClass]
    public class HandleReportAsyncTest
    {
        private Mock<IReportService> _mockReportService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IHubContext<AppHub>> _mockSignalRHub;
        private Mock<IUserRepository> _mockUserRepository;
        private ReportController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockReportService = new Mock<IReportService>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockSignalRHub = new Mock<IHubContext<AppHub>>();
            _mockUserRepository = new Mock<IUserRepository>();

            _controller = new ReportController(
                _mockReportService.Object,
                _mockNotificationService.Object,
                _mockSignalRHub.Object,
                _mockUserRepository.Object
            );
        }
        [TestMethod]
        public async Task HandleReportAsync_ModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Required");

            var handleReportDto = new HandleReportDto
            {
                ReportId = 1,
                ReportedBy = 2,
                ReportedOn = 3,
                HandleReportType = HandleReport_Type.WARNING,
                Content = ""
            };

            // Act
            var result = await _controller.HandleReportAsync(handleReportDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task HandleReportAsync_ReportServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var handleReportDto = new HandleReportDto
            {
                ReportId = 1,
                ReportedBy = 2,
                ReportedOn = 3,
                HandleReportType = HandleReport_Type.WARNING,
                Content = "Report Content"
            };

            _mockReportService
                .Setup(service => service.HandleReportAsync(handleReportDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.HandleReportAsync(handleReportDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task HandleReportAsync_BanUserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var handleReportDto = new HandleReportDto
            {
                ReportId = 1,
                ReportedBy = 2,
                ReportedOn = -1,
                HandleReportType = HandleReport_Type.BAN,
                Content = "Report Content"
            };

            _mockReportService
                .Setup(service => service.HandleReportAsync(handleReportDto))
                .ReturnsAsync(true);

            _mockUserRepository
                .Setup(repo => repo.GetByIdAsync(handleReportDto.ReportedOn))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.HandleReportAsync(handleReportDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task HandleReportAsync_BanUser_Success_ReturnsOk()
        {
            // Arrange
            var handleReportDto = new HandleReportDto
            {
                ReportId = 1,
                ReportedBy = 2,
                ReportedOn = 3,
                HandleReportType = HandleReport_Type.BAN,
                Content = "Report Content"
            };

            var user = new User { Uid = 3, Status = Common_Status.ACTIVE };

            _mockReportService
                .Setup(service => service.HandleReportAsync(handleReportDto))
                .ReturnsAsync(true);

            _mockUserRepository
                .Setup(repo => repo.GetByIdAsync(handleReportDto.ReportedOn))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(repo => repo.UpdateAsync(user))
                .ReturnsAsync(true);
            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _mockSignalRHub.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);

            // Act
            var result = await _controller.HandleReportAsync(handleReportDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task HandleReportAsync_SendNotifications_Success()
        {
            // Arrange
            var handleReportDto = new HandleReportDto
            {
                ReportId = 1,
                ReportedBy = 2,
                ReportedOn = 3,
                HandleReportType = HandleReport_Type.WARNING,
                Content = "Report Content"
            };

            _mockReportService
                .Setup(service => service.HandleReportAsync(handleReportDto))
                .ReturnsAsync(true);

            _mockNotificationService
                .Setup(service => service.AddAsync(It.IsAny<NotificationDto>()))
                .ReturnsAsync(new NotificationDto());
            // Mock the SendAsync call to ensure it's called with the expected parameters
            var mockClientProxy = new Mock<IClientProxy>();
            _mockSignalRHub.Setup(hub => hub.Clients.All).Returns(mockClientProxy.Object);

            // Act
            var result = await _controller.HandleReportAsync(handleReportDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
