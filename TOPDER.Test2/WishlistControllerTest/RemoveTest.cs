using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TOPDER.API.Controllers;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.WishlistControllerTest
{
    [TestClass]
    public class RemoveTest
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
        public async Task Remove_Successful_ReturnsOk()
        {
            // Arrange
            int customerId = 1;
            int wishlistId = 100;

            // Mock the service method to return true, indicating successful removal
            _wishlistServiceMock.Setup(s => s.RemoveAsync(wishlistId, customerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Remove(customerId, wishlistId);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Xóa nhà hàng ra khỏi Wishlist thành công", okResult.Value);
        }

        [TestMethod]
        public async Task Remove_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            int customerId = 1;
            int wishlistId = 100;

            // Mock the service method to return false, indicating item was not found or doesn't belong to the customer
            _wishlistServiceMock.Setup(s => s.RemoveAsync(wishlistId, customerId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Remove(customerId, wishlistId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.       Assert.AreEqual(404, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Wishlist item not found or does not belong to the customer.", notFoundResult.Value);
        }
    }
}
