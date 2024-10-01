using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _feedbackService.AddAsync(feedbackDto);
            if (result)
            {
                return Ok("Feedback created successfully.");
            }
            return BadRequest("Failed to create feedback.");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryCustomerPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int customerId)
        {
            var result = await _feedbackService.GetHistoryCustomerPagingAsync(pageNumber, pageSize, customerId);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Feedback with ID {id} deleted successfully.");
            }
            return NotFound($"Feedback with ID {id} not found.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _feedbackService.UpdateAsync(feedbackDto);
            if (result)
            {
                return Ok("Feedback updated successfully.");
            }
            return NotFound("Feedback not found.");
        }

        [HttpGet("customer/{restaurantId}")]
        public async Task<IActionResult> GetCustomerFeedbacks(int restaurantId,[FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int? star = null)
        {
            var result = await _feedbackService.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId, star);
            var response = new PaginatedResponseDto<FeedbackCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminFeedbacks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? star = null, [FromQuery] string? content = null)
        {
            var result = await _feedbackService.ListAdminPagingAsync(pageNumber, pageSize, star, content);

            var response = new PaginatedResponseDto<FeedbackAdminDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantFeedbacks(int restaurantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? star = null, [FromQuery] string? content = null)
        {
            var result = await _feedbackService.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, star, content);
            var response = new PaginatedResponseDto<FeedbackRestaurantDto>(result, result.PageIndex, result.TotalPages, result.HasPreviousPage, result.HasNextPage);
            return Ok(response);
        }

    }
}
