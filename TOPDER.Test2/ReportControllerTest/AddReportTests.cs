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
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ReportControllerTest
{
    [TestClass]
    public class AddReportTests
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
        public async Task AddReport_ReturnsOk_WhenReportIsSuccessfullyAdded()
        {
            // Arrange
            var reportDto = new ReportDto
            {
                ReportedBy = 1,
                ReportedOn = 1,
                ReportType = "Customer",
                Description = "Spam message",
            };

            _mockReportService.Setup(service => service.AddAsync(It.IsAny<ReportDto>())).ReturnsAsync(true); // Simulate successful report addition

            // Act
            var result = await _controller.AddReport(reportDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            var response = okResult.Value as dynamic;
        }


        [TestMethod]
        public async Task AddReport_ReturnsBadRequest_WhenReportTypeIsNull()
        {
            // Arrange: Create a report with Description set to empty string
            var reportDto = new ReportDto
            {
                ReportedBy = 1,
                ReportedOn = 1,
                ReportType = "",
                Description = "Description",  // Empty description
            };

            // Act: Call the AddReport method
            var result = await _controller.AddReport(reportDto);

            // Assert: Check if the result is a BadRequest
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Verify the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // Verify the status code is 400
        }

        [TestMethod]
        public async Task AddReport_ReturnsBadRequest_WhenDescriptionIsEmpty()
        {
            // Arrange: Create a report with Description set to empty string
            var reportDto = new ReportDto
            {
                ReportedBy = 1,
                ReportedOn = 1,
                ReportType = "Customer",
                Description = "",  // Empty description
            };

            // Act: Call the AddReport method
            var result = await _controller.AddReport(reportDto);

            // Assert: Check if the result is a BadRequest
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Verify the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // Verify the status code is 400
        }

        [TestMethod]
        public async Task AddReport_ReturnsBadRequest_WhenReportedByIsNull()
        {
            // Arrange: Create a report with Description set to empty string
            var reportDto = new ReportDto
            {
                ReportedBy = -1,
                ReportedOn = 1,
                ReportType = "Customer",
                Description = "Description",  // Empty description
            };

            // Act: Call the AddReport method
            var result = await _controller.AddReport(reportDto);

            // Assert: Check if the result is a BadRequest
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Verify the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // Verify the status code is 400
        }

        [TestMethod]
        public async Task AddReport_ReturnsBadRequest_WhenReportedOnIsNull()
        {
            // Arrange: Create a report with Description set to empty string
            var reportDto = new ReportDto
            {
                ReportedBy = 1,
                ReportedOn = 0,
                ReportType = "Customer",
                Description = "Description",  // Empty description
            };

            // Act: Call the AddReport method
            var result = await _controller.AddReport(reportDto);

            // Assert: Check if the result is a BadRequest
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);  // Verify the result is BadRequest
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode); // Verify the status code is 400
        }
    }

}
