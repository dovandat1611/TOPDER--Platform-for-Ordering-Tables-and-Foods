using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.ReportControllerTest
{
    [TestClass]
    public class GetReportsTest
    {
        private Mock<IReportService> _reportServiceMock;
        private ReportController _controller;

        [TestInitialize]
        public void Setup()
        {
            _reportServiceMock = new Mock<IReportService>();
            _controller = new ReportController(_reportServiceMock.Object); // Inject mock service
        }

        [TestMethod]
        public async Task GetReports_ReturnsOkResult_WithPaginatedReports()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            
            // Act
            var result = await _controller.GetReports(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Check that the result is Ok (200 OK)
            
        }

        [TestMethod]
        public async Task GetReports_ReturnsOkResult_WithDefaultParameters()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
           

            // Act
            var result = await _controller.GetReports(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult); // Check that the result is Ok (200 OK)
            
        }

    }
}
