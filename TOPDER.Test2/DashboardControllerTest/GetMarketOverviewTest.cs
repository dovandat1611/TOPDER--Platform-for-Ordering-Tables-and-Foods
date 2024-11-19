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
    public class GetMarketOverviewTest
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
        public async Task GetMarketOverview_ReturnsOkResult_WhenDataExists()
        {
            // Arrange
            int restaurantId = 1;
            int? filteredYear = 2024;
            var mockData = new MarketOverviewDTO
            {
                TotalInComeForYear = 10000,
                TotalInComeGrowthRateForYear = 15.5,
                OrderForYear = 500,
                OrderGrowthRateForYear = 8.3,
                MonthlyData = new List<ChartDTO>
                {
                    new ChartDTO { Month = 1, TotalOrders = 50, TotalInComes = 1000 },
                    new ChartDTO { Month = 2, TotalOrders = 45, TotalInComes = 950 }
                }
            };
            _mockDashboardService
                .Setup(service => service.GetMarketOverviewRestaurantAsync(restaurantId, filteredYear))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetMarketOverview(restaurantId, filteredYear);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(MarketOverviewDTO));
            var responseData = okResult.Value as MarketOverviewDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10000, responseData.TotalInComeForYear);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, responseData.OrderForYear);
        }

        [TestMethod]
        public async Task GetMarketOverview_ReturnsOkResult_WhenFilteredYearIsNull()
        {
            // Arrange
            int restaurantId = 1;
            int? filteredYear = null;
            var mockData = new MarketOverviewDTO
            {
                TotalInComeForYear = 10000,
                TotalInComeGrowthRateForYear = 12.5,
                OrderForYear = 600,
                OrderGrowthRateForYear = 9.0
            };
            _mockDashboardService
                .Setup(service => service.GetMarketOverviewRestaurantAsync(restaurantId, filteredYear))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetMarketOverview(restaurantId, filteredYear);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(MarketOverviewDTO));
            var responseData = okResult.Value as MarketOverviewDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(10000, responseData.TotalInComeForYear);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(600, responseData.OrderForYear);
        }

        [TestMethod]
        public async Task GetMarketOverview_HandlesInvalidRestaurantId()
        {
            // Arrange
            int restaurantId = -1; // Invalid restaurantId
            int? filteredYear = 2024;
            _mockDashboardService
                .Setup(service => service.GetMarketOverviewRestaurantAsync(restaurantId, filteredYear))
                .ThrowsAsync(new KeyNotFoundException("Restaurant not found."));

            // Act
            async Task Action() => await _controller.GetMarketOverview(restaurantId, filteredYear);

            // Assert
            await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<KeyNotFoundException>(Action);
            _mockDashboardService.Verify(service => service.GetMarketOverviewRestaurantAsync(restaurantId, filteredYear), Times.Once);
        }
    }
}
