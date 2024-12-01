using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.API.Controllers;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;
using Microsoft.AspNetCore.SignalR;
using TOPDER.Service.Hubs;

namespace TOPDER.Test2.NotificationControllerTest
{
    [TestClass]
    public class UpdateTest
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IHubContext<AppHub>> _signalRHubMock;
        private NotificationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _signalRHubMock = new Mock<IHubContext<AppHub>>();

            _controller = new NotificationController(
                _notificationServiceMock.Object,
                _signalRHubMock.Object
            );
        }


        [TestMethod]
        public async Task Update_ValidNotification_ReturnsOk()
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

            _notificationServiceMock.Setup(service => service.UpdateAsync(notificationDto))
                .ReturnsAsync(true);  // Giả lập việc thêm thông báo thành công

            // Act
            var result = await _controller.Update(notificationDto);

            // Assert
            var okResult = result as OkObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(okResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Cập nhật thông báo thành công.", okResult.Value);
        }

        [TestMethod]
        public async Task Update_InvalidNotificationModel_ReturnsBadRequest()
        {
            // Arrange
            decimal price = 50.0m;  // Giá trị price cho thử nghiệm

            var notificationDto = new NotificationDto
            {
                NotificationId = 1,  // Dữ liệu không hợp lệ (ví dụ thiếu Uid hoặc Content)
                Uid = 0,  // UID không hợp lệ
                Content = "",  // Nội dung không hợp lệ
                Type = Notification_Type.SYSTEM_ADD  // Sử dụng hằng số từ Notification_Type
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Content", "Content is required.");

            // Act
            var result = await _controller.Update(notificationDto);

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
        public async Task Update_InvalidNotificationModel_ContentNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 1,
                Uid = 1,  // UID hợp lệ
                Content = null,  // Nội dung null không hợp lệ
                Type = Notification_Type.SYSTEM_ADD
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Content", "Content is required.");

            // Act
            var result = await _controller.Update(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Content"));
        }

        [TestMethod]
        public async Task Update_InvalidNotificationModel_TypeNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 1,
                Uid = 1,  // UID hợp lệ
                Content = Notification_Content.SYSTEM_SUB(15000),  // Nội dung hợp lệ
                Type = null  // Type null không hợp lệ
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Type", "Type is required.");

            // Act
            var result = await _controller.Update(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Type"));
        }


        [TestMethod]
        public async Task Update_InvalidNotificationModel_UidNull_ReturnsBadRequest()
        {
            // Arrange
            var notificationDto = new NotificationDto
            {
                NotificationId = 1,
                Uid = 0,  // UID không hợp lệ
                Content = Notification_Content.SYSTEM_SUB(15000),  // Nội dung hợp lệ
                Type = Notification_Type.SYSTEM_ADD
            };

            // Thêm lỗi vào trạng thái model (ModelState)
            _controller.ModelState.AddModelError("Uid", "Uid is required.");

            // Act
            var result = await _controller.Update(notificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(badRequestResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var modelStateErrors = badRequestResult.Value as SerializableError;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(modelStateErrors);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(modelStateErrors.ContainsKey("Uid"));
        }
        [TestMethod]
        public async Task Update_NotificationIdInvalid_ReturnsNotFound()
        {
            var notificationDto = new NotificationDto
            {
                NotificationId = -1,  // Invalid NotificationId that doesn't exist
                Uid = 1,  // Valid UID
                Content = "Updated content",
                Type = "SYSTEM_ADD"
            };

            // Simulate that the notification with ID 9999 does not exist or belongs to another user
            _notificationServiceMock.Setup(service => service.UpdateAsync(notificationDto))
                .ReturnsAsync(false);  // Simulate update failure due to non-existing notification

            var result = await _controller.Update(notificationDto);

            var notFoundResult = result as NotFoundObjectResult;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(notFoundResult);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Không tìm thấy thông báo hoặc thông báo không thuộc về user.", notFoundResult.Value);
        }
    }
}
