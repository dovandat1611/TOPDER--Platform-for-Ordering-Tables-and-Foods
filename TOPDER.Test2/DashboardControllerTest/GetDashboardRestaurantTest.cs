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
    public class GetDashboardRestaurantTest
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
        public async Task GetDashboardRestaurant_ReturnsOkResult_WhenDataExists()
        {
            // Arrange
            int restaurantId = 1;
            var mockData = new DashboardRestaurantDto
            {
                TaskBar = new TaskBarRestaurantDTO
                {
                    TotalOrder = 100,
                    TotalIncome = 5000,
                    Star = 4,
                    RestaurantBookingStatus = true,
                    CurrentMonthIncome = new CurrentMonthIncomeDTO { CurrentMonthIncome = 1000 },
                    CurrentMonthOrder = new CurrentMonthOrderDTO { OrderGrowthRate = 10 }
                },
                OrderStatus = new OrderStatusDTO
                {
                    TotalOrder = 100,
                    Pending = 10,
                    Confirm = 20,
                    Paid = 30,
                    Complete = 35,
                    Cancel = 5
                },
                LoyalCustomers = new List<TopLoyalCustomerDTO>
                {
                    new TopLoyalCustomerDTO { CustomerId = 1, Name = "John Doe", TotalOrder = 10, TotalInCome = 100 }
                },
                CustomerAgeGroup = new CustomerAgeGroupDTO { Under18 = 10, Between18And24 = 20 },
                FeedbackStars = new FeedbackStarDTO { Star5 = 80, Star4 = 15 },
                MarketOverview = new MarketOverviewDTO
                {
                    TotalInComeForYear = 5000,
                    TotalInComeGrowthRateForYear = 10.5,
                    OrderForYear = 200,
                    OrderGrowthRateForYear = 5.0
                },
                YearsContainOrders = new List<int> { 2023, 2024 }
            };
            _mockDashboardService
                .Setup(service => service.GetDashboardRestaurantAsync(restaurantId))
                .ReturnsAsync(mockData);

            // Act
            var result = await _controller.GetDashboardRestaurant(restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult.Value);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(okResult.Value, typeof(DashboardRestaurantDto));
            var responseData = okResult.Value as DashboardRestaurantDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100, responseData.TaskBar.TotalOrder);
        }

        [TestMethod]
        public async Task GetDashboardRestaurant_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            int restaurantId = 9999;
            _mockDashboardService
                .Setup(service => service.GetDashboardRestaurantAsync(restaurantId))
                .ThrowsAsync(new KeyNotFoundException("No data found."));

            // Act
            var result = await _controller.GetDashboardRestaurant(restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("No data found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetDashboardRestaurant_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int restaurantId = 1;
            _mockDashboardService
                .Setup(service => service.GetDashboardRestaurantAsync(restaurantId))
                .ThrowsAsync(new Exception("An unexpected error occurred."));

            // Act
            var result = await _controller.GetDashboardRestaurant(restaurantId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.IsNotNull(result);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var serverErrorResult = result.Result as ObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(500, serverErrorResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("An unexpected error occurred.", serverErrorResult.Value);
        }
    }
}
