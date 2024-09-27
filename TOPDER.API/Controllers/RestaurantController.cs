using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ISendMailService _sendMailService;

        public RestaurantController(IRestaurantService restaurantService, CloudinaryService cloudinaryService, ISendMailService sendMailService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
        }

        [HttpGet("customer/service")]
        public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null, [FromQuery] string? address = null,
            [FromQuery] string? location = null, [FromQuery] int? restaurantCategory = null,
            [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? maxCapacity = null)
        {
            var result = await _restaurantService.GetItemsAsync(pageNumber, pageSize, name, address, location,
                restaurantCategory, minPrice, maxPrice, maxCapacity);

            var response = new PaginatedResponseDto<RestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpGet("customer/home")]
        public async Task<IActionResult> GetHomeItems()
        {
            var result = await _restaurantService.GetHomeItemsAsync();
            return Ok(result);
        }

        [HttpGet("customer/detail/{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                var result = await _restaurantService.GetItemAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
