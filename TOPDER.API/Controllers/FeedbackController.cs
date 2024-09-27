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

        [HttpPost("create")]
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

        [HttpGet("list/admin")]
        public async Task<IActionResult> GetAdminPaging([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _feedbackService.GetAdminPagingAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("list/customer")]
        public async Task<IActionResult> GetCustomerPaging([FromQuery] int pageNumber , [FromQuery] int pageSize, [FromQuery] int restaurantId)
        {
            var result = await _feedbackService.GetCustomerPagingAsync(pageNumber, pageSize, restaurantId);
            return Ok(result);
        }

        [HttpGet("list/history")]
        public async Task<IActionResult> GetHistoryCustomerPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int customerId)
        {
            var result = await _feedbackService.GetHistoryCustomerPagingAsync(pageNumber, pageSize, customerId);
            return Ok(result);
        }

        [HttpGet("list/restaurant")]
        public async Task<IActionResult> GetRestaurantPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int restaurantId)
        {
            var result = await _feedbackService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId);
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

        [HttpPut("update")]
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

        [HttpGet("search/customer")]
        public async Task<IActionResult> SearchCustomerPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int restaurantId, [FromQuery] int star)
        {
            var result = await _feedbackService.SearchCustomerPagingAsync(pageNumber, pageSize, restaurantId, star);
            var response = new PaginatedResponseDto<FeedbackCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpGet("search/admin")]
        public async Task<IActionResult> SearchAdminPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int star)
        {
            var result = await _feedbackService.SearchAdminPagingAsync(pageNumber, pageSize, star);

            var response = new PaginatedResponseDto<FeedbackAdminDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("search/restaurant")]
        public async Task<IActionResult> SearchRestaurantPaging([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int restaurantId, [FromQuery] int star)
        {
            var result = await _feedbackService.SearchRestaurantPagingAsync(pageNumber, pageSize, restaurantId, star);

            var response = new PaginatedResponseDto<FeedbackRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

    }
}
