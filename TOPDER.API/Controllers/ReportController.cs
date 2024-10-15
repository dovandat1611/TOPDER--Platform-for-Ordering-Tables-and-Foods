using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
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
