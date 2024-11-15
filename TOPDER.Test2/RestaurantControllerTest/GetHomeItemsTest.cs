using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.RestaurantControllerTest
{
    [TestClass]
    public class GetHomeItemsTest
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
        public async Task GetHomeItems_ReturnsOkResult_WithHomeItems()
        {
            // Arrange
            var homeItems = new RestaurantHomeDto
            {
                TopBookingRestaurants = new List<RestaurantDto>
            {
                new RestaurantDto { Uid = 1, NameRes = "Top Booking 1", TotalFeedbacks = 100, Star = 5 },
                new RestaurantDto { Uid = 2, NameRes = "Top Booking 2", TotalFeedbacks = 90, Star = 4 }
            },
                TopRatingRestaurant = new List<RestaurantDto>
            {
                new RestaurantDto { Uid = 3, NameRes = "Top Rating 1", TotalFeedbacks = 80, Star = 5 },
                new RestaurantDto { Uid = 4, NameRes = "Top Rating 2", TotalFeedbacks = 75, Star = 5 }
            },
                NewRestaurants = new List<RestaurantDto>
            {
                new RestaurantDto { Uid = 5, NameRes = "New Restaurant 1", TotalFeedbacks = 10, Star = 3 },
                new RestaurantDto { Uid = 6, NameRes = "New Restaurant 2", TotalFeedbacks = 15, Star = 4 }
            },
                Blogs = new List<BlogListCustomerDto>
            {
                new BlogListCustomerDto { BlogId = 101, Title = "Blog 1" },
                new BlogListCustomerDto { BlogId = 102, Title = "Blog 2" }
            }
            };

            _restaurantServiceMock
                .Setup(service => service.GetHomeItemsAsync())
                .ReturnsAsync(homeItems);

            // Act
            var result = await _controller.GetHomeItems();

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnedHomeItems = okResult.Value as RestaurantHomeDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnedHomeItems);

            // Validate Top Booking Restaurants
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedHomeItems.TopBookingRestaurants.Count);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Top Booking 1", returnedHomeItems.TopBookingRestaurants[0].NameRes);

            // Validate Top Rating Restaurants
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedHomeItems.TopRatingRestaurant.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, returnedHomeItems.TopRatingRestaurant[0].Star);

            // Validate New Restaurants
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedHomeItems.NewRestaurants.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("New Restaurant 2", returnedHomeItems.NewRestaurants[1].NameRes);

            // Validate Blogs
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, returnedHomeItems.Blogs.Count);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Blog 1", returnedHomeItems.Blogs[0].Title);
        }

    }
}
