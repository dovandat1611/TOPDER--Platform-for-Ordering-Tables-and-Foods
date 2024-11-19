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
    public class GetDashboardAdminTests
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
        public async Task GetDashboardAdmin_ReturnsOkResult_WhenDataExists()
        {
            // Arrange
            var dashboardData = new DashboardAdminDTO
            {
                OrderStatus = new OrderStatusDTO { Pending = 5, Complete = 10, Cancel = 2 },
                TaskBar = new TaskBarAdminDTO { TotalRestaurant = 20, TotalCustomer = 200, TotalOrder = 100, TotalIncome = 5000 },
                TopRestaurantDTOs = new List<TopRestaurantDTO>(),
                CustomerAgeGroup = new CustomerAgeGroupDTO(),
                ChartCategoryRestaurants = new List<ChartCategoryRestaurantDTO>(),
                MarketOverview = new MarketOverviewDTO(),
                YearsContainOrders = new List<int> { 2021, 2022 }
            };

            _mockDashboardService
                .Setup(service => service.GetDashboardAdminAsync())
                .ReturnsAsync(dashboardData);

            // Act
            var result = await _controller.GetDashboardAdmin();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var resultData = okResult.Value as DashboardAdminDTO;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(resultData);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, resultData.OrderStatus.Pending);
        }


        [TestMethod]
        public async Task GetDashboardAdmin_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            _mockDashboardService
                .Setup(service => service.GetDashboardAdminAsync())
                .ThrowsAsync(new KeyNotFoundException("No data found."));

            // Act
            var result = await _controller.GetDashboardAdmin();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No data found.", notFoundResult.Value);
        }


        [TestMethod]
        public async Task GetDashboardAdmin_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockDashboardService
                .Setup(service => service.GetDashboardAdminAsync())
                .ThrowsAsync(new Exception("An unexpected error occurred."));

            // Act
            var result = await _controller.GetDashboardAdmin();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);

            // Kiểm tra kiểu trả về là ObjectResult
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var serverErrorResult = result.Result as ObjectResult;

                 Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(serverErrorResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(500, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An unexpected error occurred.", serverErrorResult.Value);
        }

    }
}
