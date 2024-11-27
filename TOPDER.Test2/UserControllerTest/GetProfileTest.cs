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
using TOPDER.Service.Dtos.Admin;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.Test2.UserControllerTest
{
    [TestClass]
    public class GetProfileTest
    {
        // Khai báo các trường cho mock objects
        private Mock<IRestaurantService> _restaurantServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private Mock<IWalletService> _walletServiceMock;
        private Mock<ISendMailService> _sendMailServiceMock;
        private Mock<JwtHelper> _jwtHelperMock;
        private Mock<IAdminService> _adminServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IIdentityService> _identityServiceMock;
        private Mock<IUserOtpRepository> _userOtpRepositoryMock;

        private UserController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Khởi tạo các Mock cho dịch vụ và repository
            _restaurantServiceMock = new Mock<IRestaurantService>();
            _userServiceMock = new Mock<IUserService>();
            _customerServiceMock = new Mock<ICustomerService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _walletServiceMock = new Mock<IWalletService>();
            _sendMailServiceMock = new Mock<ISendMailService>();
            _jwtHelperMock = new Mock<JwtHelper>();
            _adminServiceMock = new Mock<IAdminService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _userOtpRepositoryMock = new Mock<IUserOtpRepository>();

            // Khởi tạo UserController với các mock objects
            _controller = new UserController(
                _restaurantServiceMock.Object,
                _cloudinaryServiceMock.Object,
                _sendMailServiceMock.Object,
                _userServiceMock.Object,
                _customerServiceMock.Object,
                _walletServiceMock.Object,
                _jwtHelperMock.Object,
                _adminServiceMock.Object,
                _userRepositoryMock.Object,
                _identityServiceMock.Object,
                _userOtpRepositoryMock.Object
            );
        }


        [TestMethod]
        public async Task GetProfile_ReturnsOkResult_WhenRoleIsCustomer()
        {
            // Arrange
            var uid = 1;
            var role = new GetRoleAndBalanceForProfileDto { Role = User_Role.CUSTOMER, WalletBalance = 100.0m };
            var customerProfile = new CustomerProfileDto
            {
                Uid = uid,
                Name = "John Doe",
                Phone = "123456789",
                Gender = "Male",
                WalletBalance = 100.0m
            };

            // Correct the setup to use _userServiceMock and _customerServiceMock
            _userServiceMock.Setup(x => x.GetRoleUserProfile(uid)).ReturnsAsync(role);
            _customerServiceMock.Setup(x => x.Profile(uid)).ReturnsAsync(customerProfile);

            // Act
            var result = await _controller.GetProfile(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);
            var returnValue = okResult.Value as CustomerProfileDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("John Doe", returnValue.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100.0m, returnValue.WalletBalance);
        }
        [TestMethod]
        public async Task GetProfile_ShouldReturnAdminProfile_WhenRoleIsAdmin()
        {
            // Arrange
            int uid = 1;
            var role = new GetRoleAndBalanceForProfileDto
            {
                Role = User_Role.ADMIN,
                WalletBalance = 500
            };

            var adminProfileDto = new AdminDto
            {
                Uid = uid,
                Name = "Admin Name",
                Phone = "987654321",
                Dob = new DateTime(1985, 5, 15),
                Image = "admin-image.jpg"
            };

            // Mock the service methods
            _userServiceMock.Setup(s => s.GetRoleUserProfile(uid)).ReturnsAsync(role);
            _adminServiceMock.Setup(s => s.Profile(uid)).ReturnsAsync(adminProfileDto);

            // Act
            var result = await _controller.GetProfile(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as AdminDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue, "Admin profile was not returned.");

            // Additional assertions for each property of AdminDto
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(uid, returnValue.Uid);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Admin Name", returnValue.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("987654321", returnValue.Phone);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(new DateTime(1985, 5, 15), returnValue.Dob);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("admin-image.jpg", returnValue.Image);
        }




        [TestMethod]
        public async Task GetProfile_ShouldReturnRestaurantProfile_WhenRoleIsRestaurant()
        {
            // Arrange
            int uid = 3;
            var role = new GetRoleAndBalanceForProfileDto
            {
                Role = User_Role.RESTAURANT,
                WalletBalance = 200
            };
            var restaurantProfileDto = new RestaurantProfileDto
            {
                Uid = uid,
                WalletBalance = 200,
                CategoryRestaurantId = 1,
                CategoryRestaurantName = "Italian",
                NameOwner = "John Doe",
                NameRes = "Pizza Place",
                Description = "Delicious pizzas",
                Subdescription = "Best in town",
                Logo = "logo.png",
                OpenTime = new TimeSpan(9, 0, 0),
                CloseTime = new TimeSpan(22, 0, 0),
                ProvinceCity = "New York",
                District = "Manhattan",
                Commune = "Downtown",
                Address = "123 Pizza St.",
                Phone = "123-456-7890",
                MaxCapacity = 50,
                Price = 20.0m
            };

            _userServiceMock.Setup(s => s.GetRoleUserProfile(uid)).ReturnsAsync(role);
            _restaurantServiceMock.Setup(s => s.Profile(uid)).ReturnsAsync(restaurantProfileDto);

            // Act
            var result = await _controller.GetProfile(uid);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as RestaurantProfileDto;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(returnValue);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(200, returnValue.WalletBalance);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Pizza Place", returnValue.NameRes);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("123 Pizza St.", returnValue.Address);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("John Doe", returnValue.NameOwner);
        }



        [TestMethod]
        public async Task GetProfile_ShouldReturnNotFound_WhenProfileNotFound()
        {
            // Arrange
            int uid = 4;
            var role = new GetRoleAndBalanceForProfileDto
            {
                Role = "CUSTOMER",
                WalletBalance = 0
            };

            _userServiceMock.Setup(s => s.GetRoleUserProfile(uid)).ReturnsAsync(role);

            // Act
            var result = await _controller.GetProfile(uid);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(404, notFoundResult.StatusCode);
          
        }

    }
}
