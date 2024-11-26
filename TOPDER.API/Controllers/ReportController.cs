using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _signalRHub;
        private readonly IUserRepository _userRepository;


        public ReportController(IReportService reportService,
            INotificationService notificationService, IHubContext<AppHub> signalRHub, IUserRepository userRepository)
        {
            _reportService = reportService;
            _notificationService = notificationService;
            _signalRHub = signalRHub;
            _userRepository = userRepository;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Report: Cusomer | Restaurant")]
        public async Task<IActionResult> AddReport([FromBody] ReportDto reportDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportService.AddAsync(reportDto);
            if (result)
                return Ok(new { Message = "Report added successfully." });

            return BadRequest(new { Message = "Failed to add report." });
        }

        [HttpGet("GetReportList")]
        [SwaggerOperation(Summary = "Danh sách Report: Admin")]
        public async Task<IActionResult> GetReports([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var paginatedReports = await _reportService.GetPagingAsync(pageNumber, pageSize);
            return Ok(paginatedReports);
        }

        [HttpPut("HandleReport")]
        [SwaggerOperation(Summary = "Tạo Report: Cusomer | Restaurant")]
        public async Task<IActionResult> HandleReportAsync([FromBody] HandleReportDto handleReportDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportService.HandleReportAsync(handleReportDto);
            if (result)
            {
                if (handleReportDto.HandleReportType == HandleReport_Type.WARNING || handleReportDto.HandleReportType == HandleReport_Type.BAN)
                {
                    if(handleReportDto.HandleReportType == HandleReport_Type.BAN)
                    {
                        var user = await _userRepository.GetByIdAsync(handleReportDto.ReportedOn);
                        if (user == null)
                        {
                            return BadRequest(new { Message = "Vô hiệu hóa tài khoản thất bại." });
                        }
                        if(user.Status == Common_Status.ACTIVE)
                        {
                            user.Status = Common_Status.INACTIVE;
                            await _userRepository.UpdateAsync(user);
                        }
                    }
                NotificationDto notificationByDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = handleReportDto.ReportedBy,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.REPORT_HANDLE_CUSTOMER(),
                    Type = Notification_Type.REPORT,
                    IsRead = false,
                };

                NotificationDto notificationOnDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = handleReportDto.ReportedBy,
                    CreatedAt = DateTime.Now,
                    Content = handleReportDto.Content,
                    Type = Notification_Type.REPORT,
                    IsRead = false,
                };

                var notificationBy = await _notificationService.AddAsync(notificationByDto);
                var notificationOn = await _notificationService.AddAsync(notificationOnDto);


                if (notificationOn != null && notificationBy != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notificationOn, notificationBy };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

                    return Ok(new { Message = "Xử lý report thành công." });

                }
            }
            return BadRequest(new { Message = "Xử lý report lỗi." });
        }



        [HttpDelete("Delete/{reportId}")]
        [SwaggerOperation(Summary = "Xóa Report: Admin")]
        public async Task<IActionResult> DeleteReport(int reportId)
        {
            var result = await _reportService.RemoveAsync(reportId);
            if (result)
                return Ok(new { Message = "Report deleted successfully." });

            return NotFound(new { Message = $"Report with ID {reportId} not found." });
        }

    }
}
