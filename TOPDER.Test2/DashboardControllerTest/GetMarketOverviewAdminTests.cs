using Microsoft.AspNetCore.Mvc;
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
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.DashboardControllerTest
{
    [TestClass]
    public class GetMarketOverviewAdminTests
    {
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IDashboardService> _mockDashboardService;
        private DashboardController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockDashboardService = new Mock<IDashboardService>();
            _controller = new DashboardController(_mockDashboardService.Object, _mockOrderRepository.Object);
        }

        [TestMethod]
        public async Task GetMarketOverviewAdmin_ReturnsOkResult_WhenDataExists()
        {
            // Arrange
            int? filteredYear = 2024;
            var mockOrders = new List<Order> // Replace with your Order class or DTO
            {
                new Order { OrderId = 1, TotalAmount = 100, CreatedAt = new DateTime(2024, 1, 15) },
                new Order { OrderId = 2, TotalAmount = 200, CreatedAt = new DateTime(2024, 2, 10) },
            };
            var mockOverview = new MarketOverviewDTO
            {
                TotalInComeForYear = 300,
                TotalInComeGrowthRateForYear = 10.5,
                OrderForYear = 2,
                OrderGrowthRateForYear = 5.0,
                MonthlyData = new List<ChartDTO>
                {
                    new ChartDTO { Month = 1, TotalOrders = 1, TotalInComes = 100 },
                    new ChartDTO { Month = 2, TotalOrders = 1, TotalInComes = 200 }
                }
            };

            _mockOrderRepository
                .Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(mockOrders.AsQueryable());

            _mockDashboardService
                .Setup(service => service.GetMarketOverviewAdminAsync(mockOrders.AsQueryable(), filteredYear))
                .ReturnsAsync(mockOverview);

            // Act
            var result = await _controller.GetMarketOverview(filteredYear);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(result);
                   Microsoft.VisualStudio.TestTools.UnitTesting.    Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(MarketOverviewDTO));
            var responseData = okResult.Value as MarketOverviewDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(300, responseData.TotalInComeForYear);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, responseData.OrderForYear);
        }

        [TestMethod]
        public async Task GetMarketOverviewAdmin_ReturnsOkResult_WhenFilteredYearIsNull()
        {
            // Arrange
            int? filteredYear = null;
            var mockOrders = new List<Order>
            {
                new Order { OrderId = 1, TotalAmount = 100, CreatedAt = new DateTime(2023, 1, 15) },
                new Order { OrderId = 2, TotalAmount = 200, CreatedAt = new DateTime(2024, 2, 10) },
            };
            var mockOverview = new MarketOverviewDTO
            {
                TotalInComeForYear = 300,
                TotalInComeGrowthRateForYear = 12.0,
                OrderForYear = 2,
                OrderGrowthRateForYear = 6.0
            };

            _mockOrderRepository
                .Setup(repo => repo.QueryableAsync())
                .ReturnsAsync(mockOrders.AsQueryable());

            _mockDashboardService
                .Setup(service => service.GetMarketOverviewAdminAsync(mockOrders.AsQueryable(), filteredYear))
                .ReturnsAsync(mockOverview);

            // Act
            var result = await _controller.GetMarketOverview(filteredYear);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
