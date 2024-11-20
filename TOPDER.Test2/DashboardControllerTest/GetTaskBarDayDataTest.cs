using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.DashboardControllerTest
{
    [TestClass]
    public class GetTaskBarDayDataTest
    {
        private Mock<IDashboardService> _mockDashboardService;
        private Mock<IOrderRepository> _mockOrderRepository;
        private DashboardController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockDashboardService = new Mock<IDashboardService>();
            _controller = new DashboardController(_mockDashboardService.Object, _mockOrderRepository.Object);
        }

        [TestMethod]
        public async Task GetTaskBarDayData_ValidRequest_ReturnsOk()
        {
            // Arrange
            var restaurantId = 1;
            var searchDay = new DateTime(2024, 11, 20); // example date

            // Create mock response
            var mockResult = new TaskBarDayRestaurantDTO
            {
               DayIncome = 111,
               DayOrders = 11
            };

            // Set up the mock service call
            _mockDashboardService.Setup(service => service.GetTaskBarDayDataAsync(restaurantId, searchDay))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetTaskBarDayData(restaurantId, searchDay);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

        }


        [TestMethod]
        public async Task GetTaskBarDayData_NullSearchDay_ReturnsOk()
        {
            // Arrange
            var restaurantId = 1;
            DateTime? searchDay = null;

            // Create mock response for null searchDay
            var mockResult = new TaskBarDayRestaurantDTO
            {
                DayIncome = 111,
                DayOrders = 11
            };

            // Set up the mock service call with null searchDay
            _mockDashboardService.Setup(service => service.GetTaskBarDayDataAsync(restaurantId, searchDay))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetTaskBarDayData(restaurantId, searchDay);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

        }

    }
}
