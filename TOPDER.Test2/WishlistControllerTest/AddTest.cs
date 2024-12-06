using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Wishlist;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.WishlistControllerTest
{
    [TestClass]
    public class AddTest
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
        public async Task Add_ValidWishlistDto_ReturnsOk()
        {
            // Arrange
            var wishlistDto = new WishlistDto
            {
                CustomerId = 1,
                RestaurantId = 1
            };

            // Mock the service method to return true (indicating successful addition)
            _wishlistServiceMock.Setup(s => s.AddAsync(wishlistDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Add(wishlistDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task Add_NullWishlistDto_ReturnsBadRequest()
        {
            // Arrange
            WishlistDto wishlistDto = null; // Null wishlistDto

            // Act
            var result = await _controller.Add(wishlistDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Add_WishlistItemAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var wishlistDto = new WishlistDto
            {
                CustomerId = 1,
                RestaurantId = 123
            };

            // Mock the service method to return false (indicating the item already exists)
            _wishlistServiceMock.Setup(s => s.AddAsync(wishlistDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Add(wishlistDto);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(conflictResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(409, conflictResult.StatusCode);
        }
    }
}
