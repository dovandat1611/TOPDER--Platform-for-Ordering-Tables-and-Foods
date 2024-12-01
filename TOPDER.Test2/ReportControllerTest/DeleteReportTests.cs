using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.ReportControllerTest
{
    [TestClass]
    public class DeleteReportTests
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
        public async Task DeleteReport_ReturnsOkResult_WhenReportIsDeleted()
        {
            // Arrange
            int reportId = 1;
            _mockReportService
                .Setup(service => service.RemoveAsync(reportId))
                .ReturnsAsync(true); // Simulate successful deletion

            // Act
            var result = await _controller.DeleteReport(reportId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Ensure the result is Ok (200 OK)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteReport_ReturnsNotFoundResult_WhenReportDoesNotExist()
        {
            // Arrange
            int reportId = -1; // Assume this ID does not exist
            _mockReportService
                .Setup(service => service.RemoveAsync(reportId))
                .ReturnsAsync(false); // Simulate deletion failure

            // Act
            var result = await _controller.DeleteReport(reportId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult); // Ensure the result is NotFound (404)
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
