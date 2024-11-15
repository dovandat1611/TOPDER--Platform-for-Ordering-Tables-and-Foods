using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class GetItemsTest
    {
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private RestaurantController _controller;

        [TestInitialize]
        public void Setup()
        {
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _controller = new RestaurantController(_restaurantServiceMock.Object, _cloudinaryServiceMock.Object); // Inject mock service
        }

        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithPaginatedResponse()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Italian", response.Items[0].CategoryName);
        }

        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithEmptyList_WhenNoResultsFound()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var emptyList = new PaginatedList<RestaurantDto>(new List<RestaurantDto>(), 0, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, response.Items.Count); // Ensure the list is empty
        }

        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullName()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
        }

        // Case 2: Test when `address` is null
        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullAddress()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.           Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
        }

        // Case 3: Test when `provinceCity` is null
        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullProvinceCity()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
                        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
        }

        // Case 4: Test when `district` is null
        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullDistrict()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
        }

        // Case 5: Test when `minPrice` and `maxPrice` are null
        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullPriceRange()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = 101,
                    CategoryName = "Italian",
                    Discount = 10,
                    Price = 100,
                    TotalFeedbacks = 50,
                    Star = 4
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
        }

        // Case 6: Test when `restaurantCategory`, `maxCapacity` are null
        [TestMethod]
        public async Task GetItems_ReturnsOkResult_WithNullRestaurantCategoryAndCapacity()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var restaurantList = new List<RestaurantDto>
            {
                new RestaurantDto
                {
                    Uid = 1,
                    Logo = "logo1.png",
                    NameRes = "Restaurant 1",
                    CategoryRestaurantId = null, // Null restaurant category
                    CategoryName = "General",
                    Discount = 5,
                    Price = 50,
                    TotalFeedbacks = 20,
                    Star = 5
                }
            };
            var paginatedList = new PaginatedList<RestaurantDto>(restaurantList, 1, pageNumber, pageSize);

            _restaurantServiceMock
                .Setup(service => service.GetItemsAsync(
                    pageNumber, pageSize, null, null, null, null, null, null, null, null, null))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetItems(pageNumber, pageSize, null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PaginatedResponseDto<RestaurantDto>;
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.IsNotNull(response);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(1, response.Items.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Restaurant 1", response.Items[0].NameRes);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(response.Items[0].CategoryRestaurantId);
        }
    }
}
