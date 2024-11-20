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
    public class GetTaskBarMonthDataTest
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
        public async Task GetTaskBarMonthData_ValidRequest_ReturnsOk()
        {
            // Arrange
            var restaurantId = 1;
            var searchMonth = new DateTime(2024, 11, 1); // example month

            // Create mock response
            var mockResult = new TaskBarMonthRestaurantDTO
            {
                CurrentMonthIncome = new CurrentMonthIncomeDTO
                {
                    CurrentMonthIncome = 5000.0,
                    IncomeGrowthRate = 10.0
                },
                CurrentMonthOrder = new CurrentMonthOrderDTO
                {
                    CurrentMonthOrder = 150,
                    OrderGrowthRate = 5.0
                }
            };

            // Set up the mock service call
            _mockDashboardService.Setup(service => service.GetTaskBarMonthDataAsync(restaurantId, searchMonth))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetTaskBarMonthData(restaurantId, searchMonth);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as TaskBarMonthRestaurantDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockResult.CurrentMonthIncome.CurrentMonthIncome, response.CurrentMonthIncome.CurrentMonthIncome);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockResult.CurrentMonthOrder.CurrentMonthOrder, response.CurrentMonthOrder.CurrentMonthOrder);
        }

       

        [TestMethod]
        public async Task GetTaskBarMonthData_NullSearchMonth_ReturnsOk()
        {
            // Arrange
            var restaurantId = 1;
            DateTime? searchMonth = null;

            // Create mock response for null searchMonth
            var mockResult = new TaskBarMonthRestaurantDTO
            {
                CurrentMonthIncome = new CurrentMonthIncomeDTO
                {
                    CurrentMonthIncome = 6000.0,
                    IncomeGrowthRate = 8.0
                },
                CurrentMonthOrder = new CurrentMonthOrderDTO
                {
                    CurrentMonthOrder = 200,
                    OrderGrowthRate = 4.0
                }
            };

            // Set up the mock service call with null searchMonth
            _mockDashboardService.Setup(service => service.GetTaskBarMonthDataAsync(restaurantId, searchMonth))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetTaskBarMonthData(restaurantId, searchMonth);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as TaskBarMonthRestaurantDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockResult.CurrentMonthIncome.CurrentMonthIncome, response.CurrentMonthIncome.CurrentMonthIncome);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(mockResult.CurrentMonthOrder.CurrentMonthOrder, response.CurrentMonthOrder.CurrentMonthOrder);
        }
    }
}
