using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.IServices;

namespace TOPDER.Test2.MenuControllerTest
{
    [TestClass]
    public class CreateNotificationTest
    {
        private Mock<INotificationService> _notificationServiceMock;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _controller = new NotificationController(_notificationServiceMock.Object);
        }

        [TestMethod]
        public async Task Create_ValidNotification_ReturnsOk()
        {
            // Arrange
            decimal price = 100.0m;  // Giả sử giá trị price là 100
            var notificationDto = new NotificationDto
            {
                NotificationId = 1,
                Uid = 123,
                Content = Notification_Content.SYSTEM_ADD(price),  // Sử dụng phương thức trong Notification_Content để tạo nội dung động
                Type = Notification_Type.SYSTEM_ADD,  // Sử dụng hằng số từ Notification_Type
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _notificationServiceMock.Setup(service => service.AddAsync(notificationDto))
                .ReturnsAsync(true);  // Giả lập việc thêm thông báo thành công

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.   Assert.AreEqual("Thêm thông báo thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task Create_InvalidNotificationModel_ReturnsBadRequest()
        {
            // Arrange
            decimal price = 50.0m;  // Giá trị price cho thử nghiệm

            var notificationDto = new NotificationDto
            {
                NotificationId = 0,  // Dữ liệu không hợp lệ (ví dụ thiếu Uid hoặc Content)
                Uid = 0,  // UID không hợp lệ
                Content = "",  // Nội dung không hợp lệ
                Type = Notification_Type.SYSTEM_ADD  // Sử dụng hằng số từ Notification_Type
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Content", "Content is required.");

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            // Kiểm tra nếu có thông báo lỗi chứa "Content is required" trong BadRequestObjectResult
            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Content"));
        }   


        [TestMethod]
        public async Task Create_NotificationAddFails_ReturnsBadRequest()
        {
            // Arrange
            decimal price = 200.0m;  // Giá trị price cho thử nghiệm

            var notificationDto = new NotificationDto
            {
                NotificationId = 1,
                Uid = 123,
                Content = Notification_Content.SYSTEM_SUB(price),  // Sử dụng phương thức trong Notification_Content để tạo nội dung động
                Type = Notification_Type.SYSTEM_SUB,  // Sử dụng hằng số từ Notification_Type
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _notificationServiceMock.Setup(service => service.AddAsync(notificationDto))
                .ReturnsAsync(false);  // Giả lập việc thêm thông báo thất bại

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Thêm thông báo thất bại.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Create_InvalidNotificationModel_ContentNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 0,
                Uid = 1,  // UID hợp lệ
                Content = null,  // Nội dung null không hợp lệ
                Type = Notification_Type.SYSTEM_ADD
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Content", "Content is required.");

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Content"));
        }

        [TestMethod]
        public async Task Create_InvalidNotificationModel_TypeNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 0,
                Uid = 1,  // UID hợp lệ
                Content = Notification_Content.SYSTEM_SUB(15000),  // Nội dung hợp lệ
                Type = null  // Type null không hợp lệ
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Type", "Type is required.");

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Type"));
        }


        [TestMethod]
        public async Task Create_InvalidNotificationModel_UidNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 0,
                Uid = 0,  // UID không hợp lệ
                Content = Notification_Content.SYSTEM_SUB(15000),  // Nội dung hợp lệ
                Type = Notification_Type.SYSTEM_ADD
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Uid", "Uid is required.");

            // Act
            var result = await _controller.Create(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Uid"));
        }
    }
}
