using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Wishlist;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.Test2.WishlistControllerTest
{
    [TestClass]
    public class GetPagingTest
    {
        private Mock<IWishlistService> _wishlistServiceMock;
        private WishlistController _controller;

        [TestInitialize]
        public void SetUp()
        {
            // Mock the IWishlistService
            _wishlistServiceMock = new Mock<IWishlistService>();

            // Initialize the controller with the mocked service
            _controller = new WishlistController(_wishlistServiceMock.Object);
        }

        [TestMethod]
        public async Task GetPaging_ValidCustomerId_ReturnsOk()
        {
            // Arrange
            int customerId = 1;
            int pageNumber = 1;
            int pageSize = 10;

            // Mock data to return
            var userWishlistDtoList = new List<UserWishlistDto>
            {
                new UserWishlistDto { },
                new UserWishlistDto {  }
            };

            // Construct the PaginatedList with proper arguments
            var paginatedResult = new PaginatedList<UserWishlistDto>(
                userWishlistDtoList,  // List of items
                pageNumber,           // Page number
                pageSize,             // Page size
                userWishlistDtoList.Count  // Total count
            );

            // Mock the service method to return paginated result
            _wishlistServiceMock.Setup(s => s.GetPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaging(customerId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPaging_NoWishlistsForCustomer_ReturnsOkWithEmptyList()
        {
            // Arrange
            int customerId = 2; // Customer with no wishlists
            int pageNumber = 1;
            int pageSize = 10;

            // Mock data to return an empty list
            var paginatedResult = new PaginatedList<UserWishlistDto>(
                new List<UserWishlistDto>(), // Empty list
                pageNumber,                  // Page number
                pageSize,                    // Page size
                0                             // Total count
            );

            // Mock the service method to return empty list
            _wishlistServiceMock.Setup(s => s.GetPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaging(customerId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPaging_InvalidCustomerId_ReturnsOkWithEmptyList()
        {
            // Arrange
            int customerId = -1; // Invalid customer ID
            int pageNumber = 1;
            int pageSize = 10;

            // Mock data to return an empty list
            var paginatedResult = new PaginatedList<UserWishlistDto>(
                new List<UserWishlistDto>(), // Empty list
                pageNumber,                  // Page number
                pageSize,                    // Page size
                0                             // Total count
            );

            // Mock the service method to return empty list
            _wishlistServiceMock.Setup(s => s.GetPagingAsync(pageNumber, pageSize, customerId))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetPaging(customerId, pageNumber, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}
